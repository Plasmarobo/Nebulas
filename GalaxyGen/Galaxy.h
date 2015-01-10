#ifndef GALAXYGEN_H
#define GALAXYGEN_H

#include <random>

#define ZLIB_WINAPI
#define BIGENDIAN

struct Color
{
	union {
		unsigned int intValue;
		struct argb
		{
			//Reverse order for intel
#ifdef BIGENDIAN
			unsigned char b;
			unsigned char g;
			unsigned char r;
			unsigned char a;
#else
			unsigned char a;
			unsigned char r;
			unsigned char g;
			unsigned char b;
#endif
		} argbValues;
	} values;
	Color()
	{
		values.intValue = 0xFF000000;
	}
	Color(unsigned int value)
	{
		values.intValue = value;
	}
	Color(unsigned char a, unsigned char r, unsigned char g, unsigned char b)
	{
		values.argbValues = { a, r, g, b };
	}
};

enum Colors {
	RED = 0,
	WHITE,
	BLUE,
	ORANGE,
	YELLOW,
	COLOR_COUNT
};

const Color gradientColors[] =
{
	Color(0x805B1D8A), 
	Color(0x80DB46CF),
	Color(0x80DB8446), 
	Color(0x80EB2409),
	Color(0x808FA63D), 
	Color(0x8069F3FA),
	Color(0x80EDFF4A),
	Color(0x801625F5)
};

const Color coronaColors[] =
{
	Color(0xFFF7B4AB),
	Color(0xFFF7F3BC),
	Color(0xFFBAEEFF),
	Color(0xFFFFD29E),
	Color(0xFFFFFECC)
};

const Color coreColors[] =
{
	Color(0xFFFF5029),
	Color(0xFFFFFFFF),
	Color(0xFF266BFF),
	Color(0xFFFFBF00),
	Color(0xFFFFEE7D)
};

class Nebula
{
protected:
	double mInitialRadius;
	unsigned long mX;
	unsigned long mY;
	unsigned long mBranchTolerance;
	unsigned int mGradientSize;
	Color mGradientPoints[5];
public:
	Nebula(unsigned long x, unsigned long y, double initialRadius, unsigned char alpha);
	Color GradientFunction(unsigned long x, unsigned long y);
	void AddRandomColor();
	void Draw();
	/*
	float d = sqrt(dx * dx + dy * dy);
	pixel = gradient - (int) (d / radius * gradient);
	new_alpha = old_alpha + the gradient formula
	*/
	void RecurseFractals(unsigned long x, unsigned long y, double radius);



};

class Galaxy
{
protected:
	unsigned long mWidth;       //Galaxy width
	unsigned long mHeight;      //Galaxy height
	//Stellar density 
	double mAverageStellarDensity;     //Zeroth level stellar density
	double mAverageStellarTemperature; //Zeroth level stellar temperature
	double mAverageStellarRadius;    //Zeroth level stellar radius

	unsigned long mBaseSeed;   // Seed from which everything is generated
	unsigned long mLayerScale; // Transition scale from layer to layer
	unsigned long mLayerCount; // Number of layers to generate
	
	std::random_device* mRandomDevice; //Random Number generator
	std::mt19937* mTwister;
	std::uniform_real_distribution<double>* mRandomDistribution;
public:

	Galaxy();
	~Galaxy();
    void GenerateLayer(int z);
	void ParseSettings(int argc, char* arguments []);
	bool CreateDirectoryTree();
	void ExportPng(unsigned int* data, int z, int slice);
	void Generate();
	void DrawStar(unsigned int* buffer, unsigned long cx, unsigned long cy, unsigned long w, unsigned long h, double radius, double temperature);
	void DrawNebula(unsigned int* buffer, unsigned long cx, unsigned long cy, unsigned long w, unsigned long h, double temperature);
	Color CoreTemperatureToColor(double temperature);
	Color CoronaTemperatureToColor(double temperature);
	Color ScaleColor(Color color, double amount);
	double GetTemperatureBrightness(double temperature);
	double Random();
};


#endif