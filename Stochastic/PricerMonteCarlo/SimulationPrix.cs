using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accord.Math;

using Stochastic.Generateurs;

namespace Stochastic.PricerMonteCarlo
{
    class SimulationPrix
    {
        /*************************************************
         ******* Attribues de la classe SimulationPrix ***
         *************************************************/
        Random rnd;
        private long NSim_; double r_; double S0_; double K_; int t_; double Sigma_;
        public SimulationPrix() { }
        public SimulationPrix(double S0, double K, int t, double sigma, double r, long NSim) 
        {
            this.S0_ = S0;
            this.K_ = K;
            this.t_ = t;
            this.Sigma_ = sigma;
            this.r_ = r;            
            this.NSim_ = NSim;
            rnd = new Random();
        }

        public Stochastic.Generateurs.LoiNormal LoiNormal
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
   

        public double[] matricePrixSimules()
        {
            double[] PrixSimul = new double[NSim_]; ;
            double NormBoxMuller; //Variable Normale centrée reduite
            double mu = (r_ - 0.5 * Math.Pow(Sigma_, 2))*t_;
            double sig = Sigma_ * Math.Sqrt(t_);

            for (int i = 0; i < NSim_; i++)
            {    
                NormBoxMuller = LoiNormal.random_normal_parBoxMuller(rnd);
                PrixSimul[i] = S0_ * Math.Exp(mu + (sig * NormBoxMuller));
                if (i < NSim_ - 1) PrixSimul[i + 1] = S0_ * Math.Exp(mu - (sig * NormBoxMuller));            
                i++;
            }
            return PrixSimul;
        }
    }
}
