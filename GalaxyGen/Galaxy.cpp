#include "Galaxy.h"
#include <png.h>
#include <pngconf.h>
#include <pnglibconf.h>
#include <fstream>
#include <ctime>
#include <math.h>

#include <direct.h>

Galaxy::Galaxy()
{
	mWidth = 2000;
	mHeight = 2000;

	mAverageStellarDensity = 0.5;
	mAverageStellarTemperature = 0.5;
	mAverageStellarRadius = 0.5;

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
		if (strcmp(arguments[iter], "-w"))
		{
			mWidth = atol(arguments[++iter]);
		}
		else if (strcmp(arguments[iter], "-h")) {
			mHeight = atol(arguments[++iter]);
		}
		else if (strcmp(arguments[iter], "-s")) {
			mBaseSeed = atol(arguments[++iter]);
			delete mRandomDistribution;
			delete mTwister;
			mTwister = new std::mt19937;
			mTwister->seed(mBaseSeed);
			mRandomDistribution = new std::uniform_real_distribution<double>(0, std::nextafter(1, DBL_MAX));
		}
		else if (strcmp(arguments[iter], "-l")) {
			mLayerCount = atol(arguments[++iter]);
		}
		else if (strcmp(arguments[iter], "-c")) {
			mLayerCount = atol(arguments[++iter]);
		}
		else if (strcmp(arguments[iter], "-r")) {
			mAverageStellarRadius = atof(arguments[++iter]);
		}
		else if (strcmp(arguments[iter], "-d")) {
			mAverageStellarDensity = atof(arguments[++iter]);
		}
		else if (strcmp(arguments[iter], "-t")) {
			mAverageStellarTemperature = atof(arguments[++iter]);
		}
		++iter;
	}
}

unsigned int* Galaxy::GenerateLayer(int z)
{
	
}

bool Galaxy::CreateDirectoryTree()
{
	char dir[128];
	for (int i = 0; i < mLayerCount; ++i)
	{
		sprintf(dir, "layer_%03d", i);
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
	sprintf(slice_name, "slice%06d", slice);
	char path[256];
	sprintf(path, "layer_%03d/slice_%s.png", z, slice_name);
	FILE *fp;
	png_structp png_ptr;
	png_infop info_ptr;
	png_bytep row;
	fp = fopen(path, "wb");
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

	png_set_IHDR(png_ptr, info_ptr, mWidth, mHeight, 8, PNG_COLOR_TYPE_RGBA, PNG_INTERLACE_NONE, PNG_COMPRESSION_TYPE_BASE, PNG_FILTER_TYPE_BASE);

	png_text title_text;
	title_text.compression = PNG_TEXT_COMPRESSION_NONE;
	title_text.key = "Title";
	title_text.text = slice_name;
	png_set_text(png_ptr, info_ptr, &title_text, 1);

	png_write_info(png_ptr, info_ptr);

	row = (png_bytep) malloc(4 * mWidth * sizeof(png_byte));
	for (int y = 0; y < mHeight; y++)
	{
		for (int x = 0; x < mWidth; x++)
		{
			memcpy(&(row[x*4]), &(data[y*mHeight + x]), 4);			
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
	for (int z = 0; z < mLayerCount; ++z)
	{
		GenerateLayer(z);
	}
}

void Galaxy::DrawStar(unsigned int* buffer, unsigned long cx, unsigned long cy, unsigned long w, unsigned long h, double radius, double temperature)
{
	double maxHyp = radius * radius;
	double hyp = 0.0;
	for (double y = floor((double) -radius); y < ceil((double) radius); y += 1.0)
	{
		for (double x = floor((double) -radius); x < ceil((double) radius); x += 1.0)
		{
			unsigned int color = 0xFF000000;
			int xi = 0;
			int yi = 0;
			if (((cy+y) >= 0) && ((cx+x) >= 0) && ((cx+x) < w) && ((cy+y) < h))
			{
				double hyp = (x*x) + (y*y);
				if (x < 0)
				{
					xi = floor(x);
				}
				else if (x > 0)
				{
					xi = ceil(x);
				}
				if (y < 0)
				{
					yi = floor(y);
				}
				else if (y > 0)
				{
					yi = ceil(y);
				}
				if (hyp < maxHyp)
				{
					if (hyp > (0.9 * maxHyp))
					{
						//Corona
						color = CoronaTemperatureToColor(temperature).values.intValue;
					}
					else if (hyp > (0.7 * maxHyp))
					{
						//Falloff
						//Max : 0.7
						//Min : 0.9
						double scalar = 0.1*(1 - (hyp - 0.7));
						color = ScaleColor(CoreTemperatureToColor(temperature), scalar).values.intValue;
					}
					else
					{
						//Core
						color = CoreTemperatureToColor(temperature).values.intValue;
					}
				}
			}
			buffer[(cy + yi)*h + (cx + xi)] = color;
		}
	}
}

void Galaxy::DrawNebula(unsigned int* buffer, unsigned long cx, unsigned long cy, unsigned long w, unsigned long h, double temperature)
{
	float d = sqrt(dx * dx + dy * dy);
	pixel = gradient - (int) (d / radius * gradient);
	new_alpha = old_alpha + the gradient formula
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
	int length = sqrt((4 ^ z)); //1, 4, 16, 64
	unsigned long layerW = mWidth * length;
	unsigned long layerH = mHeight * length;
	unsigned int* buffer = new unsigned int(layerW * layerH);
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
			unsigned long offset = (mWidth * mHeight) * ((layerH * y) + x);
			ExportPng(&(buffer[offset]), z, index);
		}
	}
}