using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Practices.Unity;

namespace ConsIoC
{
    class Program
    {
        static void Main(string[] args)
        {

            #region Unity example

            //var container = new UnityContainer();

            //container.RegisterType<ICreditCard, MasterCard>();

            ////var shopper = container.Resolve<Shopper>();
            //var shopper = container.Resolve<Shopper>();

            //shopper.Charge();
            //Console.WriteLine(shopper.ChargesForCurrentCard);

            //var shopper2 = container.Resolve<Shopper>();
            //shopper2.Charge();
            //Console.WriteLine(shopper.ChargesForCurrentCard);

            //var myClass = new object();
            #endregion


            #region IoC Manually Example

            Resolver resolver = new Resolver();
            //var shopper = new Shopper(resolver.ResolveCreditCard());

            resolver.Register<Shopper, Shopper>();
            //resolver.Register<ICreditCard, MasterCard>();
            resolver.Register<ICreditCard, Visa>();

            var shopper = resolver.Resolve<Shopper>();
            shopper.Charge();
            #endregion


            Console.Read();

        }
    }

    public class Resolver
    {
        private Dictionary<Type, Type> dependencyMap = new Dictionary<Type, Type>();

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        private object Resolve(Type typeToResolve)
        {
            Type resolvedType = null;
            try
            {
                resolvedType = dependencyMap[typeToResolve];
            }
            catch
            {
                throw new Exception(string.Format("Could not resolve type {0}", typeToResolve.FullName));
            }

            var firstConstructor = resolvedType.GetConstructors().First(); //////////   from here   /////////////
            var constructorParameters = firstConstructor.GetParameters();
            if (constructorParameters.Count() == 0)
                return Activator.CreateInstance(resolvedType);

            IList<object> parameters = new List<object>();
            foreach (var parameterToResolve in constructorParameters)
            {
                parameters.Add(Resolve(parameterToResolve.ParameterType));
            }

            return firstConstructor.Invoke(parameters.ToArray());     ///////////       to here     /////////
        }

        public void Register<TFrom, TTo>()
        {
            dependencyMap.Add(typeof(TFrom), typeof(TTo));
        }
    }

    public class Visa : ICreditCard
    {
        public string Charge()
        {
            return "Chaaaarging with the Visa!";
        }

        public int ChargeCount
        {
            get { return 0; }
        }

    }

    public class MasterCard : ICreditCard
    {
        public string Charge()
        {
            ChargeCount++;
            return "Swiping the MasterCard!";
        }
        public int ChargeCount { get; set; }

    }

    public class Shopper
    {
        private readonly ICreditCard creditCard;

        public Shopper(ICreditCard creditCard)
        {
            this.creditCard = creditCard;
        }

        public int ChargesForCurrentCard
        {
            get { return creditCard.ChargeCount; }
        }

        public void Charge()
        {
            var chargeMessage = creditCard.Charge();
            Console.WriteLine(chargeMessage);
        }
    }

    public interface ICreditCard
    {
        string Charge();

        int ChargeCount { get; }

    }




}
