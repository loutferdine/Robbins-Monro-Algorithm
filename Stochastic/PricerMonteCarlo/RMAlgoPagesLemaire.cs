using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stochastic.Generateurs;

namespace Stochastic.PricerMonteCarlo
{
    public class RMAlgoPagesLemaire
    {
        Random rnd;
        private long NSim_; double r_; double S0_; double K_; int T_; double Sigma_; type_ op_;
        public RMAlgoPagesLemaire(double S0, double K, int t, double sigma, double r, long NSim,type_ op) 
        {
            this.op_ = op;
            this.S0_ = S0;
            this.K_ = K;
            this.T_ = t;
            this.Sigma_ = sigma;
            this.r_ = r;           
            this.NSim_ = NSim;
            rnd = new Random();
        }

        /******************************************************************************
         *********************    Fonction pour le calcul de la variance     **********
         ******************************************************************************/
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
        //Fonction F^2(Xn+1 - theta)
        public double FonctionFCaree(double K, double theta)
        {
            this.K_ = K;
            double ratio1=0.0;
            double Z = LoiNormal.random_normal_parBoxMuller(rnd) - theta;
            double ST = S0_ * Math.Exp((r_ - Math.Pow(Sigma_, 2) / 2) * T_ + Sigma_ * Math.Pow(T_, 0.50) * Z);
            if (this.op_ == type_.Call)
                ratio1 = Math.Pow(Math.Max(ST - K_, 0.0), 2); 
            else if (this.op_ == type_.Put)
                ratio1 = Math.Pow(Math.Max(K_ - ST, 0.0), 2);
            else throw new InvalidOperationException("Impossible de traiter le type d'option entrer!!!!: "+this.op_);
            double ratio2 = 1;
            double Psi = ratio1 * ratio2;
            return Psi;
        }
        public double ThetaOptimalPL(double K, double Theta0, double lamda, int Niter)
        {
            List<double> Theta_ = new List<double>();            
            List<double> gamma_ = new List<double>();
            List<double> y_ = new List<double>();
            double Z = LoiNormal.random_normal_parBoxMuller(rnd);

            this.K_ = K;
            Theta_.Add(Theta0);
            gamma_.Add(1);
            y_.Add(Math.Exp(-2 * lamda * Theta_[0]) * FonctionFCaree(K_, Theta_[0]) * (2 * Theta_[0] - Z));

            for (int i = 1; i <= Niter + 1; i++)
            {
                gamma_.Add(1/ (double)(i + 1));    //gamma_n = a/(b+n)
            }
            for (int i = 0; i < Niter; i++)
            {                
                Z = LoiNormal.random_normal_parBoxMuller(rnd);
                Theta_.Add(Theta_[i] - gamma_[i + 1] * y_[i]);
                y_.Add(Math.Exp(-2 * lamda * Theta_[i+1]) * FonctionFCaree(K_, Theta_[i+1])*(2 * Theta_[i+1] - Z));                
            }
            return Theta_[Niter - 1];
        }

        //Calcul de la valeur d'une option européenne en utilisant l'algorithme de RM pour la réduction de la variance
        public double[] MCEurValeurISLemairePages(double Theta, double K) 
        {
            double[] PrixSimul = new double[NSim_];
            double Ztilde = LoiNormal.random_normal_parBoxMuller(rnd) + Theta; //Variable Normale: moyenne =tetha* , variance = 1;            
            double mu = (r_ - 0.5 * Math.Pow(Sigma_, 2)) * T_;
            double sig = Sigma_ * Math.Sqrt(T_);

            List<double> Payoff_ = new List<double>();
            List<double> Z = new List<double>(); ;
            List<double> NormalVar = new List<double>();
            List<double> Zcarre = new List<double>();

            double[] res = new double[2];
            this.K_ = K;

            for (int i = 0; i < NSim_; i++)
            {
                Ztilde = LoiNormal.random_normal_parBoxMuller(rnd) + Theta;                
                PrixSimul[i] = S0_ * Math.Exp(mu + sig * Ztilde);
                NormalVar.Add(Math.Pow(Ztilde, 2));
                if (this.op_ == type_.Call)
                    Payoff_.Add(Math.Max(PrixSimul[i] - K_, 0.0));
                else if (this.op_ == type_.Put)
                    Payoff_.Add(Math.Max(K_ - PrixSimul[i], 0.0));
                else throw new InvalidOperationException("Impossible de traiter le type d'option entrer!!!!: " + this.op_);

                Z.Add(Math.Exp(-r_ * T_)*(Math.Exp(0.5 * Math.Pow(Theta, 2) - Theta * Ztilde)) * Payoff_[i]);
                Zcarre.Add((Math.Pow(Z[i], 2)));
            }
            res[0] = Z.Average(); //La moyenne des PayOff actulisés
            res[1] = Math.Sqrt(Variance(Z, Z.Average(), 0, Z.Count));//L'ecarte type de simulation, 
            return res;
        }
        
    }
}
