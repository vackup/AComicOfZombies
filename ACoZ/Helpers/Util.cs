using System;
using System.Collections.Generic;
using System.Text;

#if WINDOWS_PHONE
using Microsoft.Phone.Tasks;
#elif IPHONE
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif

namespace ACoZ.Helpers
{
    public static class Util
    {
        public static List<T> GetValues<T>()
        {
            // TODO: mover a un lugar donde puede ser accedido por todos los posibles enums
            var currentEnum = typeof(T);
            var resultSet = new List<T>();

            //if (!currentEnum.GetTypeInfo().IsEnum)
            if (!currentEnum.IsEnum)
            {
                throw new Exception(string.Format("{0} is not an enum", currentEnum));
            }

            foreach (var value in Enum.GetValues(currentEnum))
            {
                resultSet.Add((T)value);
            }

            return resultSet;
        }

        static readonly char[] NumberBuffer = new char[16];
        public static StringBuilder AppendNumber(this StringBuilder stringBuilder, int number)
        {
            return stringBuilder.AppendNumber(number, 0);
        }

        public static StringBuilder AppendNumber(this StringBuilder stringBuilder, int number, int minDigits)
        {
            if (number < 0)
            {
                stringBuilder.Append('-');
                number = -number;
            }
            int index = 0;
            do
            {
                int digit = number % 10;
                NumberBuffer[index] = (char)('0' + digit);
                number /= 10;
                ++index;
            } while (number > 0 || index < minDigits);
            for (--index; index >= 0; --index)
            {
                stringBuilder.Append(NumberBuffer[index]);
            }
            return stringBuilder;
        }

        public static void GoToUrl(string urlAdress)
        {
#if WINDOWS
            System.Diagnostics.Process.Start(urlAdress);
#elif IPHONE
            var url = new NSUrl(urlAdress);
			 if (!UIApplication.SharedApplication.OpenUrl(url))
			 {
				using (var msg = new UIAlertView ("Not Supported", "Launching " + urlAdress + " is not supported", null, "Ok")){
						msg.Show ();
					}			     
			 }
#else
            throw new NotImplementedException();
#endif
        }
    } 
}
