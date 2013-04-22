
#include "vsAuto.h"
#include <string>
#include <string.h>
#include <iostream>
#include <sstream>
#include <fstream>

#ifdef WIN32

#include <direct.h>
#else
#include <unistd.h>
#include <sys/stat.h>
#include <sys/types.h>
#define _MAX_PATH 1024
#endif

#ifndef WIN32
#define CHDIR(file) (chdir(file))
#define GETCWD(buf,size) (getcwd(buf,size))
#define STRDUP(content) (strdup(content))
#else
#if _MSC_VER > 1400
#define CHDIR(file) (_chdir(file))
#define GETCWD(buf,size) (_getcwd(buf,size))
#define STRDUP(content) (_strdup(content))
#else
#define CHDIR(file) (chdir(file))
#define GETCWD(buf,size) (getcwd(buf,size))
#define STRDUP(content) (strdup(content))
#endif
#endif



FuncCallBack callbackFunc = NULL;
FuncCallBack2 callbackFunc2 = NULL;
StpntCallBack callbackStpnt = NULL;
BcndCallBack callbackBcnd = NULL;
IcndCallBack callbackIcnd = NULL;
FoptCallBack callbackFopt = NULL;
PvlsCallBack callbackPvls = NULL;

using namespace std;

BEGIN_AUTO_NAMESPACE

void CloseAllFiles();
int AUTO_main(int argc,char *argv[]);


int func (integer ndim, const doublereal *orig_u, const integer *icp,
		  const doublereal *orig_par, integer ijac,
		  doublereal *orig_f, doublereal *dfdu, doublereal *dfdp)
{

	doublereal* u = (doublereal*)malloc(sizeof(doublereal)*ndim);
	memcpy(u, orig_u, sizeof(doublereal)*ndim);

	doublereal* par = (doublereal*)malloc(sizeof(doublereal)*10);
	memcpy(par, orig_par, sizeof(doublereal)*10);

	doublereal* f = (doublereal*)malloc(sizeof(doublereal)*ndim);
	memset(f, 0, sizeof(doublereal)*ndim);

	if (true)
	{		
		if (callbackFunc != NULL)
		{
			callbackFunc(ndim, u, icp, par, ijac, f, dfdu, dfdp);
		}

		if (callbackFunc2 != NULL)
		{
			const FuncCallBack2 oTemp = callbackFunc2;
			oTemp(u, par, f);
		}
	}

	memcpy(orig_f, f, sizeof(doublereal)*ndim);

	free(f); 
	free(par);
	free(u);

	return 0;
}

DLL_EXPORT void STD_CALL setCallbackFunc(FuncCallBack cb)
{
	callbackFunc = cb;
}
DLL_EXPORT void STD_CALL setCallbackFunc2(FuncCallBack2 cb)
{
	callbackFunc2 = cb;
}

int stpnt (integer ndim, doublereal t,
		   doublereal *u, doublereal *par)
{
	if (callbackStpnt != NULL)
		return (*callbackStpnt)(ndim, t, u, par);
	return 0;
}

DLL_EXPORT char* STD_CALL GetTempPath()
{
#ifdef WIN32
	char *tempVar = getenv("TEMP");
	if (tempVar  == NULL)
		tempVar = "c:\\Windows\\Temp";
	return STRDUP(tempVar);
#else
	return "/tmp";
#endif
}


DLL_EXPORT char* STD_CALL getFullPath(const char* fileName)
{	
	char *tempPath =  GetTempPath(); 
	char* result = (char*) malloc(sizeof(char)*(strlen(tempPath) + strlen(fileName)));
	memset(result,0,sizeof(char)*(strlen(tempPath) + strlen(fileName)));
	strcat(result, tempPath);
	strcat(result,fileName);
	free(tempPath);
	return result;
}


DLL_EXPORT void STD_CALL setCallbackStpnt(StpntCallBack cb)
{
	callbackStpnt = cb;
}

int bcnd (integer ndim, const doublereal *par, const integer *icp,
		  integer nbc, const doublereal *u0, const doublereal *u1, integer ijac,
		  doublereal *fb, doublereal *dbc)
{
	if (callbackBcnd != NULL)
		return (*callbackBcnd)(ndim, par, icp, nbc, u0, u1, ijac, fb, dbc);
	return 0;
}

DLL_EXPORT void STD_CALL setCallbackBcnd(BcndCallBack cb)
{
	callbackBcnd = cb;
}


int icnd (integer ndim, const doublereal *par, const integer *icp,
		  integer nint, const doublereal *u, const doublereal *uold,
		  const doublereal *udot, const doublereal *upold, integer ijac,
		  doublereal *fi, doublereal *dint)
{
	if (callbackIcnd != NULL)
		return (*callbackIcnd)(ndim, par, icp, nint, u, uold, udot, upold, ijac, fi, dint);
	return 0;
}

DLL_EXPORT void STD_CALL setCallbackIcnd(IcndCallBack cb)
{
	callbackIcnd = cb;
}

int fopt (integer ndim, const doublereal *u, const integer *icp,
		  const doublereal *par, integer ijac,
		  doublereal *fs, doublereal *dfdu, doublereal *dfdp)
{
	if (callbackFopt != NULL)
		return (*callbackFopt)(ndim, u, icp, par, ijac, fs, dfdu, dfdp);
	return 0;
}

DLL_EXPORT void STD_CALL setCallbackFopt(FoptCallBack cb)
{
	callbackFopt = cb;
}

int pvls (integer ndim, const void *u,
		  doublereal *par)
{
	if (callbackPvls != NULL)
		return (*callbackPvls)(ndim, u, par);

	return 0;
}

DLL_EXPORT void STD_CALL setCallbackPvls(PvlsCallBack cb)
{
	callbackPvls = cb;
}

void clearCallbacks()
{
	callbackBcnd = NULL;
	callbackFopt = NULL;
	callbackFunc = NULL;
	callbackIcnd = NULL;
	callbackPvls = NULL;
	callbackStpnt = NULL;
}

DLL_EXPORT void STD_CALL ResetAutoLib()
{
	clearCallbacks();
}





extern int num_model_pars;
extern int num_total_pars;
extern int sysoff;
char *_sFort2 = NULL;
char *_sFort3 = NULL;
int _nFort2Length;
int _nFort3Length;


DLL_EXPORT void STD_CALL SetAutoNumParameters(int n)
{
	num_model_pars = n;
	num_total_pars = 4*n;
	sysoff = n;
}



DLL_EXPORT void STD_CALL setFort2File(char* content, int length)
{
	//char *tempPath =  GetTempPath(); 
	//stringstream fileName; fileName << tempPath << 	"/fort.2\0";	
	//free(tempPath); tempPath = strdup(fileName.str().c_str());

	static char* fileName = getFullPath("/fort.2");

	if (_sFort2 != NULL)
		free (_sFort2);

	_sFort2 = STRDUP(content);
	_nFort2Length = length;


	FILE* fp = fopen(fileName, "wb");
	fwrite(content, 1, length, fp);
	fclose(fp);
	//free(tempPath);

}

DLL_EXPORT void STD_CALL setFort3File(char* content, int length)
{
	static char* fileName = getFullPath("/fort.3");

	if (_sFort3 != NULL)
		free (_sFort3);

	_sFort3 = STRDUP(content);
	_nFort3Length = length;

	

	FILE* fp = fopen(fileName, "wb");
	fwrite(content, 1, length, fp);
	fclose(fp);

}


char *_sFort7 = NULL;
char *_sFort8 = NULL;
char *_sFort9 = NULL;
int _nFort7Length;
int _nFort8Length;
DLL_EXPORT char* STD_CALL getFort7File(int length)
{

	if (_sFort7 != NULL)
		free(_sFort7);

	length = 0;

	string sFileName(GetTempPath()); 
	sFileName = sFileName + "/fort.7";

	FILE* fp = fopen(sFileName.c_str(), "rb");
	if (fp == NULL) return NULL;
	fseek(fp,0,SEEK_END); 
	length=ftell(fp); 
	fseek(fp,0,SEEK_SET); 
	_sFort7=(char *)malloc(length);
	fread(_sFort7,length,1,fp); 
	fclose(fp);	

	return _sFort7;
}

DLL_EXPORT void STD_CALL setFort7File(char* content, int length)
{
	static char* fileName = getFullPath("/fort.7");

	if (_sFort7 != NULL)
		free (_sFort7);

	_sFort7 = STRDUP(content);
	_nFort7Length = length;


	FILE* fp = fopen(fileName, "wb");
	fwrite(content, 1, length, fp);
	fclose(fp);
}


DLL_EXPORT char* STD_CALL getFort8File(int length)
{

	if (_sFort8 != NULL)
		free(_sFort8);

	length = 0;

	string sFileName(GetTempPath()); 
	sFileName = sFileName + "/fort.8";

	FILE* fp = fopen(sFileName.c_str(), "rb");
	if (fp == NULL) return NULL;
	fseek(fp,0,SEEK_END); 
	length=ftell(fp); 
	fseek(fp,0,SEEK_SET); 
	_sFort8=(char *)malloc(length);
	fread(_sFort8,length,1,fp); 
	fclose(fp);	

	return _sFort8;
}


DLL_EXPORT void STD_CALL setFort8File(char* content, int length)
{
	static char* fileName = getFullPath("/fort.8");

	if (_sFort8 != NULL)
		free (_sFort8);

	_sFort8 = STRDUP(content);
	_nFort8Length = length;

	FILE* fp = fopen(fileName, "wb");
	fwrite(content, 1, length, fp);
	fclose(fp);
}


DLL_EXPORT char* STD_CALL getFort9File(int length)
{
	static char* fileName = getFullPath("/fort.9");
	if (_sFort9 != NULL)
		free(_sFort9);

	length = 0;

	FILE* fp = fopen(fileName, "rb");
	if (fp == NULL) return NULL;
	fseek(fp,0,SEEK_END); 
	length=ftell(fp); 
	fseek(fp,0,SEEK_SET); 
	_sFort9=(char *)malloc(length);
	fread(_sFort9,length,1,fp); 
	fclose(fp);	

	return _sFort9;
}


char* _lastMessage = NULL;

DLL_EXPORT char* GetLastMessage()
{
	return _lastMessage;
}

DLL_EXPORT void STD_CALL CallAuto()
{	
	char *oldDir = GETCWD(NULL, 0);
	try
	{
		int argc = 1;
		char *argv[] = { "vsAuto" }	;
		CHDIR(GetTempPath());
		LibAuto::AUTO_main(argc, argv);	
		//delete[] argv;
	}
	catch (const char* message)
	{
		if (_lastMessage != NULL)
			free(_lastMessage);
		_lastMessage = (char*)malloc(sizeof(char)*strlen(message));
		strcpy(_lastMessage, message);
	}
	catch(...)
	{
	}
	LibAuto::CloseAllFiles();
	clearCallbacks();
	CHDIR(oldDir);
	free(oldDir);
}



END_AUTO_NAMESPACE

