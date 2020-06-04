using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stochastic.Generateurs;

namespace Stochastic.PricerMonteCarlo
{
    public class MCBasketOption
    {
        Random rnd;
        private long NSim_; double r_; double K_; int t_; List<double> Sigma_; List<double> S0_;       
        public MCBasketOption(List<double> S0, double K, int t, List<double> sigma, double r, long NSim)
        {            
            this.S0_ = S0;
            this.K_ = K;
            this.t_ = t;
            this.Sigma_ = sigma;
            this.r_ = r;            
            this.NSim_ = NSim;
            rnd = new Random();            
        }

        /******************************************************************************
         * *******************    Fonction pour le calcul de la variance     **********
         * ****************************************************************************/
        public static double Variance(List<double> values, double mean, int start, int end)
        {
            double variance = 0;
            for (int i = start; i < end; i++)
            {
                variance += Math.Pow((values[i] - mean), 2);
            }
            int n = end - start;
            if (start > 0) n -= 1;
            return variance / (n - 1);
        }
        /******************************************************************************
         * ************ Valeur d'une option basket avec Monte-Carlo simple ************
         * ****************************************************************************/
        public double[] MCBasketOptionVal(type_ op)
        {
            List<double>  Payoff_;
            double[,]ST_;
            Payoff_ = new List<double>();
            ST_ = new double[NSim_, S0_.Count];
            double NormBoxMuller;
            double[] res = new double[2];           //Tableau résultat
            switch (op)
            {
                case type_.Call:
                    for (int j=0;j<NSim_;j++)
                    {
                       double som=0;
                       for(int i=0;i<S0_.Count;i++){                           
                           NormBoxMuller = LoiNormal.random_normal_parBoxMuller(rnd);                           
                           ST_[j, i] = S0_[i] * Math.Exp((r_ - 0.5 * Math.Pow(Sigma_[i], 2)) * t_ + Sigma_[i] * Math.Sqrt(t_) * NormBoxMuller);
                           som += ST_[j, i];
                        }
                       Payoff_.Add(Math.Exp(-r_ * t_) * Math.Max((som / S0_.Count - K_), 0.0));
                    }
                    break;
                case type_.Put:
                    for (int j = 0; j < NSim_; j++)
                    {
                        double som = 0;
                        for (int i = 0; i < S0_.Count; i++)
                        {
                            NormBoxMuller = LoiNormal.random_normal_parBoxMuller(rnd);                            
                            ST_[j, i] = S0_[i] * Math.Exp((r_ - 0.5 * Math.Pow(Sigma_[i], 2)) * t_ + Sigma_[i] * Math.Sqrt(t_) * NormBoxMuller);
                            som += ST_[j, i];
                        }
                        Payoff_.Add(Math.Exp(-r_ * t_) * Math.Max((K_ - som / S0_.Count), 0.0));                 
                    }
                    break;
                default:
                    throw new InvalidOperationException("Impossible de traiter le type d'option entrer!!!!: " + op);
            }
            res[0] = Payoff_.Average();     //La valeur de l'option, Esperance des payoff actualisés
            res[1] = Math.Sqrt(Variance(Payoff_, res[0], 0, Payoff_.Count));//L'ecarte type de simulation,                        
            return res;
        }
        /******************************************************************************
         * ************         Valeur d'une option basket avec    ********************
         * ***************   la méthode des variables antithétiques *******************
         * ****************************************************************************/
        public double[] MC_AntitBasketOptionVal(type_ op) 
        {
            List<double> Payoff_;
            double[,] ST_;
            double[] res = new double[2];           //Tableau résultat
            Payoff_ = new List<double>();
            ST_ = new double[NSim_, S0_.Count];
            double NormBoxMuller;
            
            switch (op)
            {
                case type_.Call:
                    for (int j = 0; j < NSim_; j++)
                    {
                        double som = 0;
                        for (int i = 0; i < S0_.Count; i++)
                        {
                            NormBoxMuller = LoiNormal.random_normal_parBoxMuller(rnd);
                            ST_[j, i] = S0_[i] * Math.Exp((r_ - 0.5 * Math.Pow(Sigma_[i], 2)) * t_ + Sigma_[i] * Math.Sqrt(t_) * NormBoxMuller);
                            if (j < NSim_ - 1)
                            {
                                ST_[j+1, i] = S0_[i] * Math.Exp((r_ - 0.5 * Math.Pow(Sigma_[i], 2)) * t_ + Sigma_[i] * Math.Sqrt(t_) * NormBoxMuller);
                            }
                            som += ST_[j, i];
                        }
                        j++;
                        Payoff_.Add(Math.Exp(-r_ * t_) * Math.Max((som / S0_.Count - K_), 0.0));
                    }
                    break;
                case type_.Put:
                    for (int j = 0; j < NSim_; j++)
                    {
                        double som = 0;
                        for (int i = 0; i < S0_.Count; i++)
                        {
                            NormBoxMuller = LoiNormal.random_normal_parBoxMuller(rnd);
                            ST_[j, i] = S0_[i] * Math.Exp((r_ - 0.5 * Math.Pow(Sigma_[i], 2)) * t_ + Sigma_[i] * Math.Sqrt(t_) * NormBoxMuller);
                            if (j < NSim_ - 1)
                            {
                                ST_[j + 1, i] = S0_[i] * Math.Exp((r_ - 0.5 * Math.Pow(Sigma_[i], 2)) * t_ + Sigma_[i] * Math.Sqrt(t_) * NormBoxMuller);
                            }
                            som += ST_[j, i];
                        }
                        j++;
                        Payoff_.Add(Math.Exp(-r_ * t_) * Math.Max((K_ - som / S0_.Count), 0.0));
                    }
                    break;
                default:
                    throw new InvalidOperationException("Impossible de traiter le type d'option entrer!!!!: " + op);
            }
            res[0] = Payoff_.Average();     //La valeur de l'option, Esperance des payoff actualisés
            res[1] = Math.Sqrt(Variance(Payoff_, res[0], 0, Payoff_.Count));//L'ecarte type de simulation,                        
            return res;
        }

        /******************************************************************************
         * ************ Valeur d'une option basket avec la méthode de réduction *******
         ****************   de la variance Robbins Monro (Arouna)   *******************
         * ****************************************************************************/
        public double[] MC_RM_ArounaBasketOptionVal(type_ op)
        {
            //RobbinsMonroAlgorithme RM;
            List<double> Payoff_;
            double[,] ST_;
            double[] res = new double[2];           //Tableau résultat
            Payoff_ = new List<double>();
            ST_ = new double[NSim_, S0_.Count];
            double NormBoxMuller;

            switch (op)
            {
                case type_.Call:                    
                    for (int j = 0; j < NSim_; j++)
                    {
                        double som = 0;
                        for (int i = 0; i < S0_.Count; i++)
                        {
                            NormBoxMuller = LoiNormal.random_normal_parBoxMuller(rnd);
                            ST_[j, i] = S0_[i] * Math.Exp((r_ - 0.5 * Math.Pow(Sigma_[i], 2)) * t_ + Sigma_[i] * Math.Sqrt(t_) * NormBoxMuller);
                            if (j < NSim_ - 1)
                            {
                                ST_[j + 1, i] = S0_[i] * Math.Exp((r_ - 0.5 * Math.Pow(Sigma_[i], 2)) * t_ + Sigma_[i] * Math.Sqrt(t_) * NormBoxMuller);
                            }
                            som += ST_[j, i];
                        }
                        j++;
                        Payoff_.Add(Math.Exp(-r_ * t_) * Math.Max((som / S0_.Count - K_), 0.0));
                    }
                    break;
                case type_.Put:
                    for (int j = 0; j < NSim_; j++)
                    {
                        double som = 0;
                        for (int i = 0; i < S0_.Count; i++)
                        {
                            NormBoxMuller = LoiNormal.random_normal_parBoxMuller(rnd);
                            ST_[j, i] = S0_[i] * Math.Exp((r_ - 0.5 * Math.Pow(Sigma_[i], 2)) * t_ + Sigma_[i] * Math.Sqrt(t_) * NormBoxMuller);
                            if (j < NSim_ - 1)
                            {
                                ST_[j + 1, i] = S0_[i] * Math.Exp((r_ - 0.5 * Math.Pow(Sigma_[i], 2)) * t_ + Sigma_[i] * Math.Sqrt(t_) * NormBoxMuller);
                            }
                            som += ST_[j, i];
                        }
                        j++;
                        Payoff_.Add(Math.Exp(-r_ * t_) * Math.Max((K_ - som / S0_.Count), 0.0));
                    }
                    break;
                default:
                    throw new InvalidOperationException("Impossible de traiter le type d'option entrer!!!!: " + op);
            }
            res[0] = Payoff_.Average();     //La valeur de l'option, Esperance des payoff actualisés
            res[1] = Math.Sqrt(Variance(Payoff_, res[0], 0, Payoff_.Count));//L'ecarte type de simulation,                        
            return res;
        }
    }
}
