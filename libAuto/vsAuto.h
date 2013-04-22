#pragma once

#include "auto_f2c.h"
#include "auto.h"

#define BEGIN_AUTO_NAMESPACE namespace LibAuto {
#define END_AUTO_NAMESPACE }

#ifdef WIN32
#define DLL_EXPORT extern "C" __declspec(dllexport)
#define STD_CALL  __stdcall
#else 
#define DLL_EXPORT extern "C"
#define STD_CALL 
#endif


typedef  int (STD_CALL *FuncCallBack)(integer ndim, const doublereal *u, const integer *icp,
          const doublereal *par, integer ijac,
          doublereal *f, doublereal *dfdu, doublereal *dfdp);
typedef  void (STD_CALL *FuncCallBack2)(const doublereal *u, const doublereal *par, doublereal *f);

typedef  int (STD_CALL *StpntCallBack)(integer ndim, doublereal t,
           doublereal *u, doublereal *par);

typedef  int (STD_CALL *BcndCallBack)(integer ndim, const doublereal *par, const integer *icp,
          integer nbc, const doublereal *u0, const doublereal *u1, integer ijac,
          doublereal *fb, doublereal *dbc);


typedef  int (STD_CALL *IcndCallBack)(integer ndim, const doublereal *par, const integer *icp,
          integer nint, const doublereal *u, const doublereal *uold,
          const doublereal *udot, const doublereal *upold, integer ijac,
          doublereal *fi, doublereal *dint);

typedef  int (STD_CALL *FoptCallBack)(integer ndim, const doublereal *u, const integer *icp,
          const doublereal *par, integer ijac,
          doublereal *fs, doublereal *dfdu, doublereal *dfdp);

typedef  int (STD_CALL *PvlsCallBack)(integer ndim, const void *u,
          doublereal *par);

BEGIN_AUTO_NAMESPACE

DLL_EXPORT void STD_CALL setCallbackFunc2(FuncCallBack2 cb);
DLL_EXPORT void STD_CALL setCallbackFunc(FuncCallBack cb);
DLL_EXPORT void STD_CALL setCallbackStpnt(StpntCallBack cb);
DLL_EXPORT void STD_CALL setCallbackBcnd(BcndCallBack cb);
DLL_EXPORT void STD_CALL setCallbackIcnd(IcndCallBack cb);
DLL_EXPORT void STD_CALL setCallbackFopt(FoptCallBack cb);
DLL_EXPORT void STD_CALL setCallbackPvls(PvlsCallBack cb);
DLL_EXPORT void STD_CALL ResetAutoLib();
DLL_EXPORT void STD_CALL CallAuto();
DLL_EXPORT void STD_CALL SetAutoNumParameters(int n);

DLL_EXPORT void STD_CALL setFort2File(char* content, int length);
DLL_EXPORT void STD_CALL setFort3File(char* content, int length);
DLL_EXPORT char* STD_CALL getFort7File(int length);
DLL_EXPORT char* STD_CALL getFort8File(int length);
DLL_EXPORT char* STD_CALL getFort9File(int length);

END_AUTO_NAMESPACE
