using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using Accord.Math;

using Stochastic.PricerMonteCarlo;
using Stochastic.Generateurs;
using Stochastic.PricerFormulesExacte;
/********************************************************
 * ************ Projet simulation numérique *************
/* ***************************************************************************************************************
 * Pour la partie intégration des objets sous excel il suffit de générer le projet InterfaceExcelAddIn ***********
 * Et faire ajouter l'automation correpondant dans le fichier excel, l'option macro complémentaire de excel,  ****
 * le serveurs Automation vous trouver le dll 'InterfaceExcelAddIn.Implementer' générer par le projet  ***********
 ** **************************************************************************************************************/

namespace Stochastic
{
    class Program
    {
        enum Choix { MC_Anti, MC_IS };
        static void Main(string[] args)
        {

            Random rnd = new Random();

            BSEurOption Eur;
            RobbinsMonroAlgorithme RM_Put;
            RobbinsMonroAlgorithme RM_Call;
            MCEuropOption MCEur;
            RMAlgoPagesLemaire PL_Call;
            RMAlgoPagesLemaire PL_Put;            

            double S0 = 100;
            double K=95;
            double r=0.05;
            int T = 1;
            double sigma = 0.15;
            double theta0 = 1.5;
            double lamda = 30;
            long Nsim = 2000;
            int Ntier = 500000;
            Eur = new BSEurOption(S0, K, r, T, sigma);
            RM_Put = new RobbinsMonroAlgorithme(S0, K, T, sigma, r, Nsim,type_.Put);
            RM_Call = new RobbinsMonroAlgorithme(S0, K, T, sigma, r, Nsim, type_.Call);
            PL_Call = new RMAlgoPagesLemaire(S0, K, T, sigma, r, Nsim,type_.Call);
            PL_Put = new RMAlgoPagesLemaire(S0, K, T, sigma, r, Nsim, type_.Put);
            MCEur = new MCEuropOption(S0, K, T, sigma, r, Nsim);
            double z = LoiNormal.random_normal_parBoxMuller(rnd);
            double thetaOptArouna;
            double thetaPL;
            /***********************************************
             * ********** Valeur d'un call Européen ********
             * **********************************************/
            Console.WriteLine("============================================================");
            Console.WriteLine("====================Call Européen===========================");
            Console.WriteLine("============================================================");
            Console.WriteLine("Valeur exacte d'un Call (avec formules de BS)= " + Eur.callBlackScholes());
            Console.WriteLine("\nValeur d'un Call avec réduction de la variance par var ANti= " + MCEur.MCEuropOptionVal(type_.Call)[0]);
            Console.WriteLine("\nVariance IS(Variables Antithetique)= " + MCEur.MCEuropOptionVal(type_.Call)[1]);
            thetaOptArouna = RM_Call.ThetaOptimalRMArouna(K, theta0, Ntier);
            Console.WriteLine("\nTheta optimal avec Robbins Monro [Arouna]=  " + thetaOptArouna);
            Console.WriteLine("\nValeur d'un Call avec réduction de la variance IS= " + +RM_Call.MCEurValeurImportanceSampling(thetaOptArouna, K)[0]);
            Console.WriteLine("\nVariance IS(Anouna RM algo)= " + +RM_Call.MCEurValeurImportanceSampling(thetaOptArouna, K)[1]);
            thetaPL = PL_Call.ThetaOptimalPL(105, 0.3, 30, 10);
            Console.WriteLine("\nTheta RMPagesLemaire =  " + thetaPL);
            Console.WriteLine("\nValeur d'un Call avec (Algo RM PagesLemaire) =  " + PL_Call.MCEurValeurISLemairePages(thetaPL, K)[0]);
            Console.WriteLine("\nVariance IS(PagesLemaire RM algo) =  " + PL_Call.MCEurValeurISLemairePages(thetaPL, K)[1]);
                     

            /***************************************
             ********* Test options basket *********
             ***************************************/
            Console.WriteLine("\n\n============================================================");
            Console.WriteLine("====================Options Basket===========================");
            Console.WriteLine("============================================================");
            List<double> S0_ = new List<double>();
            List<double> Sigma_ = new List<double>();
            for (int i = 0; i < 3; i++)
            {
                S0_.Add(94+i*4);
                Sigma_.Add(0.25 + i * 0.02);                                
            }
            MCBasketOption bsket = new MCBasketOption(S0_, K, T, Sigma_, r, Nsim);
            MCRMBasketOption rmbask = new MCRMBasketOption(S0_, K, T, Sigma_, r, Nsim,type_.Call);
            Console.WriteLine("\n\n prix Basket Call MC_ Var Anti= " + bsket.MC_AntitBasketOptionVal(type_.Call)[0]);
            Console.WriteLine("\n\n prix Basket Put MC_ Var Anti= " + bsket.MC_AntitBasketOptionVal(type_.Put)[0]);

            double thetarmBasket = rmbask.ThetaOptimalRMArouna(105, 1.5, 100000);
            Console.WriteLine("\n\n Theta optimal avec RM de l'option Basket = " + thetarmBasket);            

            Console.ReadLine();
        }
    }
}
