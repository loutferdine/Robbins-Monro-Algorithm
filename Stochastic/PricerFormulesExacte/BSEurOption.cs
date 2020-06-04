using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Stochastic.Generateurs;

namespace Stochastic.PricerFormulesExacte
{
    public class BSEurOption
    {
        private double S, K, r, t, sigma;

        private double Delta_, Gamma_, Theta_, Rho_, Vega_;

        public BSEurOption(){}
        public BSEurOption(double _S, double _K, double _r, double _t, double _sigma)
        {
            S = _S; K = _K; r = _r; t = _t; sigma = _sigma;
        }
        LoiNormal loi = new LoiNormal();
        //Calcul de d1 et d2
        public double d1()
        {
            return (Math.Log(S / K) + r * t) / (sigma * Math.Sqrt(t)) + 0.5 * sigma * Math.Sqrt(t);
        }
        public double d2()
        {
            return d1() - (sigma * Math.Sqrt(t));
        }
        // Méthode calculant le prix d'un call d'une option par la formule de Black-Scholes
        public double callBlackScholes()
        {
            return S * loi.Phi(d1()) - K * Math.Exp(-r * t) * loi.Phi(d2());
        }

        // Méthode calculant le prix d'un put d'une option par la formule de Black-Scholes
        public double putBlackScholes()
        {
            return K * Math.Exp(-r * t) * loi.Phi(-d2()) - S * loi.Phi(-d1());
        }

        // Méthode calculant l'ensemble des dérivées partielles d'une option par la formule de Black-Scholes
        public void grecsBlackScholes()
        {
            Delta_ = loi.Phi(d1());
            Gamma_ = loi.n(d1()) / (S * sigma * Math.Sqrt(t));
            Theta_ = -(S * sigma * loi.n(d1())) / (2 * Math.Sqrt(t)) - r * K * Math.Exp(-r * t) * loi.Phi(d2());
            Vega_ = S * Math.Sqrt(t) * loi.n(d1());
            Rho_ = K * t * Math.Exp(-r * t) * loi.Phi(d2());
        }

        public double Delta
        {
            get { return Delta_; }
            set { Delta_ = value; }
        }
        public double Gamma
        {
            get { return Gamma_; }
            set { Gamma_ = value; }
        }
        public double Theta
        {
            get { return Theta_; }
            set { Theta_ = value; }
        }
        public double Rho
        {
            get { return Rho_; }
            set { Rho_ = value; }
        }
        public double Vega
        {
            get { return Vega_; }
            set { Vega_ = value; }
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

        public Stochastic.PricerMonteCarlo.type_ type_
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
    }
}
