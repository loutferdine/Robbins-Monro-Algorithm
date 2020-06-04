using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;


namespace InterfaceExcelAddIn
{
    [ComVisible(true)]
    [Guid("828d27b7-389d-4d41-bcfc-656abd11136e")]
    public interface IFonctions
    {
        double EuropOptionCallRM(double S0, double K, int t, double sigma, double r, long NSim, int Niter, double theta0);
        double ErreurRM_Call(double S0, double K, int t, double sigma, double r, long NSim, int Niter, double theta0);

        double EuropOptionCallRMPLemaire(double S0, double K, int t, double sigma, double r, long NSim, int Niter, double theta0, double lamda);
        double ErreurRMPLemaire_Call(double S0, double K, int t, double sigma, double r, long NSim, int Niter, double theta0, double lamda);


        double EuropOptionANTICall(double S0, double K, int t, double sigma, double r, long NSim);
        double ErreurANTIT_Call(double S0, double K, int t, double sigma, double r, long NSim);


        double EuropOptionCallExacte(double S0, double K, int t, double sigma, double r);
        
        double BasketOptionANTICall(Excel.Range S0range, double K, int t, Excel.Range sigmaRange, double r, long NSim);
        double ErreurBasketOptionANTICall(Excel.Range S0range, double K, int t, Excel.Range sigmaRange, double r, long NSim);


        double EuropOptionPutRM(double S0, double K, int t, double sigma, double r, long NSim, int Niter, double theta0);
        double EuropOptionPutRMPLemaire(double S0, double K, int t, double sigma, double r, long NSim, int Niter, double theta0, double lamda);

        double EuropOptionANTIPut(double S0, double K, int t, double sigma, double r, long NSim);
        double EuropOptionPutExacte(double S0, double K, int t, double sigma, double r);
        
    }
}
