#include "auto_f2c.h"
#include "auto_c.h"
#include "vsAuto.h"


BEGIN_AUTO_NAMESPACE;

/* ---------------------------------------------------------------------- */
/* ---------------------------------------------------------------------- */
/*   pp2 :    Basic computations for continuous dynamical systems */
/* ---------------------------------------------------------------------- */
/* ---------------------------------------------------------------------- */
/* ---------------------------------------------------------------------- */
/* ---------------------------------------------------------------------- */
int STD_CALL func1 (integer ndim, const doublereal *u, const integer *icp,
          const doublereal *par, integer ijac,
          doublereal *f, doublereal *dfdu, doublereal *dfdp) {

  //doublereal e;
  //
  //e = exp(-par[2] * u[0]);
  //f[0] = par[1] * u[0] * (1 - u[0]) - u[0] * u[1] - par[0] * (1 - e);
  //f[1] = -u[1] + par[3] * u[0] * u[1];
  

	double BP[3];
	double GP[7];

      

      GP[0] = par[0];
      GP[1] = par[1];
      GP[2] = par[2];
      GP[3] = par[3];
      GP[4] = par[4];
      GP[5] = par[5];
      GP[6] = par[6];
      BP[0] = par[7];
      BP[1] = par[8];
      BP[2] = par[9];


	  double rate[4];

      rate[0] = GP[0];
      rate[1] = GP[1]*u[0];
      rate[2] = (GP[2]*u[0]-GP[3]*u[1])*(1+GP[4]*pow(u[1],GP[5]));
      rate[3] = GP[6]*u[1];

      f[0] =   + rate[0] - rate[1] - rate[2];
      f[1] =   + rate[2] - rate[3];
        


  return 0;
} 
/* ---------------------------------------------------------------------- */
/* ---------------------------------------------------------------------- */
int STD_CALL stpnt1 (integer ndim, doublereal t,
           doublereal *u, doublereal *par) {

  //par[0] = (double)0.;
  //par[1] = (double)3.;
  //par[2] = (double)5.;
  //par[3] = (double)3.;

  //u[0] = (double)0.;
  //u[1] = (double)0.;

      par[0] = 1.0;
      par[1] = 0.0;
      par[2] = 1.0;
      par[3] = 0.0;
      par[4] = 1.0;
      par[5] = 3.0;
      par[6] = 5.0;
      par[7] = 1.0;
      par[8] = 0.0;
      par[9] = 0.0;

	  u[0] = 0.992063492063492;
      u[1] = 0.2;

  return 0;
}

END_AUTO_NAMESPACE;

using namespace LibAuto;

int main (int argc, char* argv[])
{

	setCallbackFunc(func1);
	setCallbackStpnt(stpnt1);
	CallAuto();

}