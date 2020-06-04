using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stochastic.Generateurs;

namespace Stochastic.PricerMonteCarlo
{
    class MCRMBasketOption
    {
        Random rnd;
        private long NSim_; double r_; List<double> S0_; double K_; int t_; List<double> Sigma_; type_ op_;
        //Constructeur de l'objet MCRMBasketOption.
        public MCRMBasketOption(List<double> S0, double K, int t, List<double> sigma, double r, long NSim, type_ op) 
        {
            this.op_ = op;
            this.S0_ = S0;
            this.K_ = K;
            this.t_ = t;
            this.Sigma_ = sigma;
            this.r_ = r;           
            this.NSim_ = NSim;
            rnd = new Random();
        }
        
        //Fonction Psi(theta_n,Ztilde_n+1) = Y_n+1.    
        public double FonctionPSI_(double K, double theta)
        {   
            this.K_ = K;
            double ratio1=0.0;
            double Z = LoiNormal.random_normal_parBoxMuller(rnd);
            List<double> ST = new List<double>();
            for (int i = 0; i < S0_.Count; i++)
            {
                ST.Add(S0_[i] * Math.Exp((r_ - Math.Pow(Sigma_[i], 2) / 2) * t_ + Sigma_[i] * Math.Pow(t_, 0.50) * Z));
            }
            if (this.op_ == type_.Call)
                ratio1 = Math.Exp(-r_ * t_) * Math.Pow(Math.Max(ST.Average() - K_, 0.0), 2); //attention Math.Exp(-2*r_ * t_)
            else if (this.op_ == type_.Put)
                ratio1 = Math.Exp(-r_ * t_) * Math.Pow(Math.Max(K_ - ST.Average(), 0.0), 2); //attention Math.Exp(-2*r_ * t_)
            else throw new InvalidOperationException("Impossible de traiter le type d'option entrer!!!!: " + this.op_);
            double ratio2 = (theta - Z) * Math.Exp(-0.5 * theta * (2 * Z - theta));
            double Psi = ratio1 * ratio2;
            return Psi;
        }        
        //Algorithme de robbinsMonro
        public double ThetaOptimalRMArouna(double K , double Theta0, int Niter) 
        {
            double x1 = -2;
            double x2 = 2;            
            List<int> rho_ = new List<int>();
            List<double> u_ = new List<double>();
            List<double> Theta_ = new List<double>();
            List<double> y_ = new List<double>();
            List<double> gamma_ = new List<double>();            
            Theta_.Add(Theta0);
            double x = LoiNormal.random_normal_parBoxMuller(rnd);
            y_.Add(FonctionPSI_(K_, Theta_[0]));
            rho_.Add(0);
            gamma_.Add(1);
            u_.Add(50);

            for (int i = 1; i <= Niter + 1; i++)
            {
                gamma_.Add(1 / (double)(i + 1));                                  //gamma_n = a/(b+n)
                u_.Add(Math.Sqrt((1 / 6) * Math.Log((double)i + 1)) + u_[0]);     //U_n = sqrt(1/6*ln(n))+U_0
            }            
            for (int i = 0; i <= Niter; i++)
            {
                rho_.Add(0);
                y_.Add(FonctionPSI_(K, Theta_[i]));
                double xn = 0;
                if (i >= 1)
                {
                    rho_[i] = rho_[i - 1];
                    if (Math.Abs(Theta_[i - 1] - gamma_[i] * y_[i]) > u_[rho_[i - 1]])
                    {
                        rho_[i] = rho_[i] + 1;
                    }
                }
                if (rho_[i] % 2 == 0) { xn = x1; }
                else { xn = x2; }

                if (Math.Abs(Theta_[i] - gamma_[i + 1] * y_[i + 1]) <= u_[rho_[i]])
                {
                    Theta_.Add(Theta_[i] - gamma_[i + 1] * y_[i + 1]);
                }
                else { Theta_.Add(xn); }
            }
            return Theta_[Niter-1];
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
            return variance / (n-1);
        }

        //Calcul de la valeur d'une option basket en utilisant l'algorithme de RM pour la réduction de la variance
        public double[] MC_RM_BasketOptionVal(double Theta, double K) 
        {
            List<double> Payoff_;
            double[,] ST_;            
            List<double> Z;
            double Ztilde = LoiNormal.random_normal_parBoxMuller(rnd) + Theta;
            Payoff_ = new List<double>();           //Payoff de l'option à chaque trajectoire
            ST_ = new double[NSim_, S0_.Count];     //ST simulées            
            Z = new List<double>();                 //
            double[] res = new double[2];           //Tableau résultat

            if (this.op_ == type_.Call){
                    for (int j = 0; j < NSim_; j++)
                    {
                        double som = 0;                        
                        for (int i = 0; i < S0_.Count; i++)
                        {
                            Ztilde = LoiNormal.random_normal_parBoxMuller(rnd) + Theta;
                            ST_[j, i] = S0_[i] * Math.Exp((r_ - 0.5 * Math.Pow(Sigma_[i], 2)) * t_ + Sigma_[i] * Math.Sqrt(t_) * Ztilde);
                            som += ST_[j, i];
                        }
                        Payoff_.Add(Math.Max((som / S0_.Count - K_), 0.0));
                        Z.Add(Math.Exp(-r_ * t_) * (Math.Exp(0.5 * Math.Pow(Theta, 2) - Theta * Ztilde)) * Payoff_[j]); 
                    }
            }
            else if(this.op_ == type_.Put){                
                    for (int j = 0; j < NSim_; j++)
                    {
                        double som = 0;
                        for (int i = 0; i < S0_.Count; i++)
                        {
                            Ztilde = LoiNormal.random_normal_parBoxMuller(rnd) + Theta;
                            ST_[j, i] = S0_[i] * Math.Exp((r_ - 0.5 * Math.Pow(Sigma_[i], 2)) * t_ + Sigma_[i] * Math.Sqrt(t_) * Ztilde);
                            som += ST_[j, i];
                        }
                        Payoff_.Add(Math.Max((K_ - som / S0_.Count), 0.0));
                        Z.Add(Math.Exp(-r_ * t_) * (Math.Exp(0.5 * Math.Pow(Theta, 2) - Theta * Ztilde)) * Payoff_[j]); 
                    }
            }
            else throw new InvalidOperationException("Impossible de traiter le type d'option entrer!!!!: ");
            res[0] = Z.Average();                                    //La valeur de l'option, Esperance des payoff actualisés
            res[1] = Math.Sqrt(Variance(Z, Z.Average(), 0, Z.Count));//L'ecarte type de simulation, 
            return res;
        }
    }
}
