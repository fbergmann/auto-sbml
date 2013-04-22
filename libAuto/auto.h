
#ifndef __AUTO_H__
#define __AUTO_H__

#ifdef __cplusplus
#define BEGIN_AUTO_NAMESPACE namespace LibAuto {
#define END_AUTO_NAMESPACE }
#else
#define BEGIN_AUTO_NAMESPACE
#define END_AUTO_NAMESPACE
#endif

/* IS THIS THING NEEDED FOR FORTRAN VERSION IN A FILE CALLED AUTO.H?

C-----------------------------------------------------------------------
C-----------------------------------------------------------------------
C NOTE : Do not change num_model_pars, NODES, KREDO, NIAP, NRAP
C-----------------------------------------------------------------------
C-----------------------------------------------------------------------
C
      PARAMETER (NDIMX=12,NCOLX=4,NTSTX=100,NBCX=NDIMX+3,NINTX=6,
     *           num_model_pars=36,NBIFX=20,NUZRX=20,
     *           NODES=1,KREDO=1,NIAP=41,NRAP=19)
C-----------------------------------------------------------------------
C-----------------------------------------------------------------------
*/

#endif
