using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Excel = Microsoft.Office.Interop.Excel;

using Stochastic.PricerMonteCarlo;
using Stochastic.Generateurs;
using Stochastic.PricerFormulesExacte;

namespace InterfaceExcelAddIn
{    
    [ComVisible(true)]
    [Guid("bb7c9ec8-ecfc-4258-97d8-f9bcd3cdb714")]
    [ProgId("InterfaceExcelAddIn.Implementer")]
    [ComDefaultInterface(typeof(IFonctions))]
    [ClassInterface(ClassInterfaceType.None)]

    public class Implementer:IFonctions
    {
        public Implementer()
        {
        }
        Random rnd1 = new Random();
        BSEurOption Eur;
        RobbinsMonroAlgorithme RM;
        RMAlgoPagesLemaire PL_Call;
        RMAlgoPagesLemaire PL_Put;
        MCEuropOption MCEur;          
        MCBasketOption MCAntitBasket ;
        List<double> thetaOpt;

        /***********************************************************************************************************************
         ******Calcul de la valeur d'un call Européen avec réduction de la  variance par l'algorithme Robbins Monro (Aroun)*****
         *******************************      Méthode d'Echantillonnage préférentiel     ***************************************
         ***********************************************************************************************************************/
        public double EuropOptionCallRM(double S0, double K, int t, double sigma, double r, long NSim, int Niter, double theta0)
        {            
            thetaOpt = new List<double>();
            RM = new RobbinsMonroAlgorithme(S0, K, t, sigma, r, NSim,type_.Call);
            thetaOpt.Add(RM.ThetaOptimalRMArouna(K, theta0, Niter));            
            return RM.MCEurValeurImportanceSampling(thetaOpt[0], K)[0];                   
        }
        /*******************************Erreur de simulation***********************************/
        public double ErreurRM_Call(double S0, double K, int t, double sigma, double r, long NSim, int Niter, double theta0)
        {
            thetaOpt = new List<double>();
            RM = new RobbinsMonroAlgorithme(S0, K, t, sigma, r, NSim, type_.Call);
            thetaOpt.Add(RM.ThetaOptimalRMArouna(K, theta0, Niter));
            return (1.96*RM.MCEurValeurImportanceSampling(thetaOpt[0], K)[1]/Math.Sqrt(NSim));
        }
        /******************************************************************************************************************************
         ******Calcul de la valeur d'un call Européen avec réduction de la  variance par l'algorithme Robbins Monro (Pâgès Lemaire)****
         ******************************      Méthode d'Echantillonnage préférentiel     ***********************************************
         ******************************************************************************************************************************/
        public double EuropOptionCallRMPLemaire(double S0, double K, int t, double sigma, double r, long NSim, int Niter, double theta0,double lamda)
        {
            PL_Call = new RMAlgoPagesLemaire(S0, K, t, sigma, r, NSim,type_.Call);
            double thetaPL = PL_Call.ThetaOptimalPL(K, theta0, lamda, Niter);
            return PL_Call.MCEurValeurISLemairePages(thetaPL, K)[0];
        }
        /*******************************Erreur de simulation***********************************/
        public double ErreurRMPLemaire_Call(double S0, double K, int t, double sigma, double r, long NSim, int Niter, double theta0, double lamda)
        {
            PL_Call = new RMAlgoPagesLemaire(S0, K, t, sigma, r, NSim, type_.Call);
            double thetaPL = PL_Call.ThetaOptimalPL(K, theta0, lamda, Niter);
            return (1.96*PL_Call.MCEurValeurISLemairePages(thetaPL, K)[1]/ Math.Sqrt(NSim));
        }
        //1.96 * RM_Put.MCEurValeurImportanceSampling(thetaOpt[0], 105)[1] / Math.Sqrt(100000);
        /********************************************************************************************************************
        *****Calcul de la valeur d'un call Européen avec réduction de la  variance par la ***********************************
        **************************     Méthode des variables antithétiques      *********************************************
        *********************************************************************************************************************/
        public double EuropOptionANTICall(double S0, double K, int t, double sigma, double r, long NSim)
        {            
            MCEur = new MCEuropOption(S0, K, t, sigma, r, NSim);
            return MCEur.MCEuropOptionVal(type_.Call)[0];
        }
        /*******************************Erreur de simulation***********************************/ 
        public double ErreurANTIT_Call(double S0, double K, int t, double sigma, double r, long NSim)
        {
            MCEur = new MCEuropOption(S0, K, t, sigma, r, NSim);
            double err = (1.96 * MCEur.MCEuropOptionVal(type_.Call)[1] / Math.Sqrt(NSim));
            return err;
        }
        /*********************************************************************************************
        *****Calcul de la valeur exacte d'une option Européenne (Call et Put) avec *******************
        ***************      Les formules de Black Scholes         ***********************************        
        **********************************************************************************************/
        //La valeur d'un call Européen
        public double EuropOptionCallExacte(double S0, double K, int t, double sigma, double r)
        {
            Eur = new BSEurOption(S0, K, r, t, sigma);
            return Eur.callBlackScholes();
        }
        //La valeur d'un put Européen
        public double EuropOptionPutExacte(double S0, double K, int t, double sigma, double r)
        {
            Eur = new BSEurOption(S0, K, r, t, sigma);
            return Eur.putBlackScholes();
        }
        /********************************************************************************************************************
        **
        *********************************************************************************************************************/
        public double EuropOptionPutRM(double S0, double K, int t, double sigma, double r, long NSim, int Niter, double theta0)
        {
            thetaOpt = new List<double>();
            RM = new RobbinsMonroAlgorithme(S0, K, t, sigma, r, NSim,type_.Put);            
            thetaOpt.Add(RM.ThetaOptimalRMArouna(K, theta0, Niter));            
            return RM.MCEurValeurImportanceSampling(thetaOpt[0], K)[0];
        }
        public double EuropOptionPutRMPLemaire(double S0, double K, int t, double sigma, double r, long NSim, int Niter, double theta0, double lamda)
        {
            PL_Put = new RMAlgoPagesLemaire(S0, K, t, sigma, r, NSim, type_.Put);
            double thetaPL = PL_Put.ThetaOptimalPL(K, theta0, lamda, Niter);
            return PL_Put.MCEurValeurISLemairePages(thetaPL, K)[0];
        }
        public double EuropOptionANTIPut(double S0, double K, int t, double sigma, double r, long NSim)
        {            
            MCEur = new MCEuropOption(S0, K, t, sigma, r, NSim);
            return MCEur.MCEuropOptionVal(type_.Put)[0];
        }
        /********************************************************************************************* 
         ******** Fonction returne permet de convertir le Rangen en List<double>  ********************
         ********************************************************************************************/
        private List<double> ConvertToDoubleList(Excel.Range range)
        {
            var result = new List<double>();
            foreach (Excel.Range row in range.Rows)
            {
                Excel.Range cell = (Excel.Range)row.Cells[1, 1];
                double value = 0;
                if (cell.Value2 != null)
                {
                    double.TryParse(cell.Value2.ToString(), out value);
                }
                result.Add(value);
            }
            return result;
        }
        /********************************************************************************************************************
        *****   Calcul de la valeur d'une option basket (Call) avec réduction de la  variance par la ************************
        **************************     Méthode des variables antithétiques      *********************************************
        *********************************************************************************************************************/
        public double BasketOptionANTICall(Excel.Range S0range, double K, int t, Excel.Range sigmaRange, double r, long NSim)
        {
            List<double> S0 = new List<double>();
            List<double> sigma = new List<double>();
            S0 = ConvertToDoubleList(S0range);
            sigma = ConvertToDoubleList(sigmaRange);
            MCAntitBasket = new MCBasketOption(S0, K, t, sigma,r, NSim);
            return MCAntitBasket.MC_AntitBasketOptionVal(type_.Call)[0];
        }
        /**************************************************************************************
         *******************************Erreur de simulation***********************************
         **************************************************************************************/
        public double ErreurBasketOptionANTICall(Excel.Range S0range, double K, int t, Excel.Range sigmaRange, double r, long NSim)
        {
            List<double> S0 = new List<double>();
            List<double> sigma = new List<double>();
            S0 = ConvertToDoubleList(S0range);
            sigma = ConvertToDoubleList(sigmaRange);
            MCAntitBasket = new MCBasketOption(S0, K, t, sigma, r, NSim);
            return (1.96 *MCAntitBasket.MC_AntitBasketOptionVal(type_.Call)[1]/Math.Sqrt(NSim));
        }

        #region COM Related
        internal const string Guid = "C426EC79-2F31-44bf-BB0A-BE9A357FA5B3";
        const string SubKeyName = @"CLSID\{" + Guid + @"}\Programmable";

        [ComRegisterFunctionAttribute]
        public static void RegisterFunction(Type type)
        {

            Registry.ClassesRoot.CreateSubKey(
              GetSubKeyName(type, "Programmable"));

            RegistryKey key = Registry.ClassesRoot.OpenSubKey(

              GetSubKeyName(type, "InprocServer32"), true);

            key.SetValue("",

              System.Environment.SystemDirectory + @"\mscoree.dll",

              RegistryValueKind.String);
        }

        [ComUnregisterFunctionAttribute]
        public static void UnregisterFunction(Type type)
        {
            Registry.ClassesRoot.DeleteSubKey(
            GetSubKeyName(type, "Programmable"), false);
        }

        private static string GetSubKeyName(Type type,

          string subKeyName)
        {

            System.Text.StringBuilder s =

              new System.Text.StringBuilder();

            s.Append(@"CLSID\{");

            s.Append(type.GUID.ToString().ToUpper());

            s.Append(@"}\");

            s.Append(subKeyName);

            return s.ToString();

        }
        #endregion
    }
}
