using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using ExtensionMethods;

namespace Stochastic.Generateurs
{
    enum boxForm { Polaire, cartesienne };

    public class LoiNormal
    {    
        public LoiNormal() { }
        // Fonction retournant un nombre aléatoire compris entre 0 et 1 
        public double random_uniform_0_1()
        {
            Random rnd = new Random();
            return rnd.NextDouble(); 
        }

        // Fonction calculant une loi normale centrée réduite 
        public double n(double x)
        {
            return (1.0 / Math.Sqrt(2.0 * Math.PI)) * Math.Exp(-0.5 * x * x);
        }

        // Fonction retournant à un nombre normal aléatoire en utilisant la forme cartésienne de Box-Muller
        public static double random_normal_parBoxMuller(Random rnd)
        {
            double u, v;
            double s;
            double Ratio;
            do
            {
               u = 2.0 * rnd.NextDouble() - 1.0;
               v = 2.0 * rnd.NextDouble() - 1.0;
               s = Math.Pow(u, 2) + Math.Pow(v, 2);
            }
            while (s >= 1 || s == 0);
            Ratio = Math.Sqrt((-2.0 * Math.Log(s)) / s);
            return (u * Ratio);
            /*
            u = rnd.NextDouble();
            v = rnd.NextDouble();
            while (u == 0)
            {
                u = rnd.NextDouble();
            }
            s = Math.Cos(2 * Math.PI * v) * Math.Sqrt(-2 * Math.Log(u));
            return s;*/
        }

        // Fonction d'approximation de la fonction de distribution cumulative normale: Approximation de Abramowitz & Stegun
        public double Phi(double z)
        {
            if (z > 6.0)
                return 1.0;
            if (z < -6.0)
                return 0.0;
            double b1 = 0.31938153;
            double b2 = -0.356563782;
            double b3 = 1.781477937;
            double b4 = -1.821255978;
            double b5 = 1.330274429;
            double p = 0.2316419;
            double c2 = 0.3989423;
            double a = Math.Abs(z);
            double t = 1.0 / (1.0 + a * p);
            double b = c2 * Math.Exp((-z) * (z / 2.0));
            double n = ((((b5 * t + b4) * t + b3) * t + b2) * t + b1) * t;
            n = 1.0 - b * n;
            if (z < 0.0)
                n = 1.0 - n;
            return n;
        }
		
    }
}
