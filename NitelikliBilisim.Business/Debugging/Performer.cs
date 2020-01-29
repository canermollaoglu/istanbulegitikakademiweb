using System.Diagnostics;
using System.Text;

namespace NitelikliBilisim.Business.Debugging
{
    public class Performer
    {
        private Stopwatch _watch;
        private static int _timesInvoked = 0;
        private static double _timePassed = 0.0;

        public Performer()
        {
            _timesInvoked++;
            _watch = new Stopwatch();
            _watch.Start();
        }

        public void Watch(string method)
        {
            _watch.Stop();
            var time = _watch.ElapsedMilliseconds / 1000.0;
            _timePassed += time;
            var message = $"{method} {time}s";
            var indicator = _createIndicator(message.Length);
            Debug.WriteLine($"{indicator}\n{message}\n{indicator}");
        }

        public static int TimesInvoked => _timesInvoked;

        public static double TimePassed => _timePassed;

        private static string _createIndicator(int len)
        {
            var stringBuilder = new StringBuilder(len);
            for (int i = 0; i < len; i++)
                stringBuilder.Append("_");
            return stringBuilder.ToString();
        }
    }
}