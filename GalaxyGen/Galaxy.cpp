#include "Galaxy.h"
extern "C"
{
#include <png.h>
#include <pngconf.h>
}
#include <fstream>
#include <ctime>
#include <math.h>
#include <algorithm>
#include <direct.h>

Galaxy::Galaxy()
{
	mWidth = 2000;
	mHeight = 2000;

	mAverageStellarDensity = 0.1;
	mAverageStellarTemperature = 0.5;
	mAverageStellarRadius = 4.0;

	mBaseSeed = time(0);
	mLayerScale = 4;
	mLayerCount = 3;

	mTwister = new std::mt19937();
	mTwister->seed(mBaseSeed);
	mRandomDistribution = new std::uniform_real_distribution<double>(0, std::nextafter(1, DBL_MAX));
}

Galaxy::~Galaxy()
{
	delete mRandomDistribution;
	delete mTwister;
}

void Galaxy::ParseSettings(int argc, char* arguments[])
{
	int iter = 0;
	while (iter < argc)
	{
		if (strcmp(arguments[iter], "-w") == 0)
		{
			mWidth = atol(arguments[++iter]);
		}
		else if (strcmp(arguments[iter], "-h") == 0) {
			mHeight = atol(arguments[++iter]);
		}
		else if (strcmp(arguments[iter], "-s") == 0) {
			mBaseSeed = atol(arguments[++iter]);
			delete mRandomDistribution;
			delete mTwister;
			mTwister = new std::mt19937;
			mTwister->seed(mBaseSeed);
			mRandomDistribution = new std::uniform_real_distribution<double>(0, std::nextafter(1, DBL_MAX));
		}
		else if (strcmp(arguments[iter], "-l") == 0) {
			mLayerCount = atol(arguments[++iter]);
		}
		else if (strcmp(arguments[iter], "-c") == 0) {
			mLayerCount = atol(arguments[++iter]);
		}
		else if (strcmp(arguments[iter], "-r") == 0) {
			mAverageStellarRadius = atof(arguments[++iter]);
		}
		else if (strcmp(arguments[iter], "-d") == 0) {
			mAverageStellarDensity = atof(arguments[++iter]);
		}
		else if (strcmp(arguments[iter], "-t") == 0) {
			mAverageStellarTemperature = atof(arguments[++iter]);
		}
		++iter;
	}
}

bool Galaxy::CreateDirectoryTree()
{
	char dir[128];
	for (int i = 0; i < mLayerCount; ++i)
	{
		sprintf_s(dir, "layer_%03d", i);
		if (_mkdir(dir) == ENOENT)
		{
			return false;
		}
	}
	return true;
}

void Galaxy::ExportPng(unsigned int* data, int z, int slice)
{
	char slice_name[128];
	sprintf_s(slice_name, "slice%06d", slice);
	char path[256];
	sprintf_s(path, "layer_%03d/slice_%s.png", z, slice_name);
	fprintf(stdout, "Exported to %s\n", path);
	FILE *fp;
	png_structp png_ptr;
	png_infop info_ptr;
	png_bytep row;
	fopen_s(&fp, path, "wb");
	if (fp == NULL)
	{
		fprintf(stderr, "Could not open %s for writing\n", path);
		return;
	}
	png_ptr = png_create_write_struct(PNG_LIBPNG_VER_STRING, NULL, NULL, NULL);
	if (png_ptr == NULL)
	{
		fprintf(stderr, "Could not allocate write struct\n");
		return;
	}
	info_ptr = png_create_info_struct(png_ptr);
	if (info_ptr == NULL)
	{
		fprintf(stderr, "Could not allocate info struct\n");
		return;
	}
	png_init_io(png_ptr, fp);

	png_set_IHDR(png_ptr, 
		info_ptr, 
		mWidth, mHeight, 
		8, 
		PNG_COLOR_TYPE_RGBA, 
		PNG_INTERLACE_NONE, 
		PNG_COMPRESSION_TYPE_DEFAULT, 
		PNG_FILTER_TYPE_DEFAULT);

	png_write_info(png_ptr, info_ptr);

	row = (png_bytep) malloc(4 * mWidth * sizeof(png_byte));
	for (int y = 0; y < mHeight; y++)
	{
		for (int x = 0; x < mWidth; x++)
		{
			memcpy(&(row[x*4]), &(data[(mWidth * mHeight * slice)+(y*mWidth) + x]), 4);			
		}
		png_write_row(png_ptr, row);
	}
	png_write_end(png_ptr, NULL);

	fclose(fp);
	png_free_data(png_ptr, info_ptr, PNG_FREE_ALL, -1);
	png_destroy_write_struct(&png_ptr, (png_infopp) NULL);
	free(row);
}

void Galaxy::Generate()
{
	CreateDirectoryTree();
	for (int z = 0; z < mLayerCount; ++z)
	{
		GenerateLayer(z);
	}
}

void Galaxy::DrawStar(unsigned int* buffer, unsigned long cx, unsigned long cy, unsigned long w, unsigned long h, double radius, double temperature)
{
	unsigned long interior = floor(radius * 0.6);
	long length = ceil(radius);
	unsigned long hyp = pow(radius, 2);
	unsigned long distance = 0;
	unsigned long x = 0;
	unsigned long y = 0;
	unsigned int color = 0x00000000;
	for (long y = -length; y < length; ++y)
	{
		for (long x = -length; x < length; ++x)
		{
			//Determine radial distance from any corner
			double corner_dist = pow((length/2) - abs(x),2) + pow((length/2) - abs(y), 2);
			distance = pow(x, 2) + pow(y, 2);



			
			if ((distance < interior) || ((x == 0) && (y == 0)))
			{
				color = CoreTemperatureToColor(temperature).values.intValue;
			}
			else
			{
				double magnitude = pow((double)x/(double)length,2.0) + pow((double)y/(double)length,2.0);
				if (magnitude > 1)
				{
					color = 0x00000000;
				}
				else
				{
					Color corona = CoreTemperatureToColor(temperature);
					corona.values.argbValues.a = 255.0 * (1.0 - magnitude);
					color = corona.values.intValue;
				}
			}
			
			if (((cy + y) < h) && ((cy + y) > 0) && ((cx + x) < w) && ((cx + x) > 0) && (color != 0x00000000))
			{
				buffer[((cy + y) * w) + (cx + x)] = color;
			}
		}
	}
}

Color Galaxy::CoreTemperatureToColor(double temperature)
{
	
	int index = floor(COLOR_COUNT * temperature);
	return ScaleColor(coreColors[index], GetTemperatureBrightness(temperature));
}

Color Galaxy::CoronaTemperatureToColor(double temperature)
{
	
	int index = floor(COLOR_COUNT * temperature);
	return ScaleColor(coronaColors[index], GetTemperatureBrightness(temperature));
}

Color Galaxy::ScaleColor(Color color, double amount)
{
	color.values.argbValues.r *= amount;
	color.values.argbValues.g *= amount;
	color.values.argbValues.b *= amount;
	return color;
}

double Galaxy::GetTemperatureBrightness(double temperature)
{
	double fraction = 1.0 / COLOR_COUNT;
	while (temperature > fraction)
	{
		temperature -= fraction;
	}

	return temperature/fraction;
}

double Galaxy::Random()
{
	return (*mRandomDistribution)(*mTwister);
}

void Galaxy::GenerateLayer(int z)
{
	int length = sqrt((pow(4, z))); //1, 4, 16, 64
	unsigned long layerW = mWidth * length;
	unsigned long layerH = mHeight * length;
	unsigned int* buffer = new unsigned int[layerW * layerH];
	for (unsigned long k = 0; k < (layerW * layerH); ++k)
	{
		buffer[k] = 0x00000000;
	}
	double density = 0;
	double starCount = 0;
	unsigned long x = 0;
	unsigned long y = 0;
	double radius = 0.0;
	double temperature = 0.0;
	while (density < mAverageStellarDensity)
	{
		x = layerW * Random();
		y = layerH * Random();
		radius = 2 * mAverageStellarRadius * Random();
		temperature = 2 * mAverageStellarTemperature * Random();
		DrawStar(buffer, x, y, layerW, layerH, radius, temperature);
		starCount += 1.0;
		density = starCount / (layerW * layerH);
	}
	for (int y = 0; y < length; ++y)
	{
		for (int x = 0; x < length; ++x)
		{
			unsigned long index = (y * length) + x;
			ExportPng(buffer, z, index);
		}
	}
	delete [] buffer;
}