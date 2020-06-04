using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stochastic.Generateurs;

namespace Stochastic.PricerMonteCarlo
{
    
    public enum type_ {Call, Put}; 
    public class MCEuropOption
    {
        private long NSim_; double r_; double S0_; double K_; int t_; double Sigma_;
        Random rnd;
        public MCEuropOption() { }
        public MCEuropOption(double S0, double K, int t, double sigma, double r, long NSim)
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
            return variance / (n-1);
        }
        /*********************************************************************************************************
         * Fonction return à la valeur d'une option européenne type=Call ou Put
         * return à une vecteur de taille 2: première composante de la table contenant la valeur de l'option
         * Deuxième composante: l'erreur de la simulation MC
        **********************************************************************************************************/
        public double[] MCEuropOptionVal(type_ op)
        {            
            List<double>ST_ , Payoff_, SquareEsperance;
            double mu = (r_ - 0.5 * Math.Pow(Sigma_, 2)) * t_;
            double sig = Sigma_ * Math.Sqrt(t_);
            double NormBoxMuller; //Variable Normale centrée reduite
            ST_ = new List<double>();               //Tableau pour stocker les valeurs de ST simuler
            Payoff_ = new List<double>();           //Tableau des PayOff 
            SquareEsperance = new List<double>();   //Esperance des crées
            double[] res = new double[2];           //Tableau résultat

            for (int i = 0; i < NSim_; i++)
            {
                NormBoxMuller = LoiNormal.random_normal_parBoxMuller(rnd);
                ST_.Add(S0_ * Math.Exp(mu + sig * NormBoxMuller));
                if (i < NSim_ - 1) ST_.Add(S0_ * Math.Exp(mu - (sig * NormBoxMuller)));
                i++;
            }
            switch(op){                
                case type_.Call:
			        for(int i=0;i<NSim_;i++){                        				        
				        Payoff_.Add(Math.Exp(-r_*t_)*Math.Max((ST_[i] - K_ ), 0.0));
                        SquareEsperance.Add(Math.Pow(Payoff_[i], 2));
			        }
                    break;
                case type_.Put:
                    for(int i=0;i<NSim_;i++){				        
				        Payoff_.Add(Math.Exp(-r_*t_)*Math.Max((K_ - ST_[i]), 0.0));
                        SquareEsperance.Add(Math.Pow(Payoff_[i], 2));
			        }		       
                    break;
                default:
                    throw new InvalidOperationException("Impossible de traiter le type d'option entrer!!!!: " + op);
            }
            res[0] = Payoff_.Average();     //La valeur de l'option, Esperance des payoff actualisés
            res[1] = Math.Sqrt(Variance(Payoff_,res[0],0,Payoff_.Count));//L'ecarte type de simulation,                        
            return 	res;
        }        
    }
}
