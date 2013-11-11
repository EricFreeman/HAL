using System.Collections.Generic;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using HAL.Framework.Modules;

namespace HAL.Framework
{
    public class Computer
    {
        #region Private Properties

        private readonly SpeechRecognitionEngine _recognizer;

        private List<IModule> _modules { get; set; }
        private List<IModule> _possibleModules { get; set; }

        private readonly Grammar _startGrammar;
        private readonly Grammar _endGrammar;

        private const string _computerName = "HAL";
        private const string _userName = "Eric";

        private const string _stopListening = "stop listening";
        private const string _stopListeningMessage = "Goodbye " + _userName;

        #endregion

        #region Constructor

        public Computer()
        {
            _recognizer = new SpeechRecognitionEngine();
            _recognizer.SetInputToDefaultAudioDevice();

            _modules = new List<IModule>();
            _possibleModules = new List<IModule>();

            _startGrammar = CreateStartGrammar();
            _endGrammar = CreateEndGrammar();

            _recognizer.LoadGrammar(_startGrammar);
            _recognizer.LoadGrammar(_endGrammar);

            _recognizer.SpeechRecognized += recognizer_SpeechRecognized;
            _recognizer.SpeechHypothesized += recognizer_SpeechHypothesized;
            _recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }

        #endregion

        #region Setup

        private Grammar CreateStartGrammar()
        {
            var b = new GrammarBuilder();
            b.Append(_computerName);

            return new Grammar(b);
        }

        private Grammar CreateEndGrammar()
        {
            var b = new GrammarBuilder();
            b.Append("stop listening");

            return new Grammar(b);
        }

        #endregion

        #region Module Support

        public bool LoadModule(IModule m)
        {
            if (_possibleModules.Contains(m)) return false;

            _possibleModules.Add(m);
            return true;
        }

        public bool UnloadModule(IModule m)
        {
            if (!_possibleModules.Contains(m)) return false;

            _possibleModules.Remove(m);
            return true;
        }

        public void LoadAllModules()
        {
            foreach (var m in _possibleModules)
            {
                _modules.Add(m);
                if (!_recognizer.Grammars.Contains(m.Grammar)) _recognizer.LoadGrammar(m.Grammar);
            }
        }

        public void UnloadAllModules()
        {
            foreach (var m in _modules)
            {
                if(_recognizer.Grammars.Contains(m.Grammar)) _recognizer.UnloadGrammar(m.Grammar);
            }

            _modules.Clear();
        }

        #endregion

        #region Helper Methods

        private void Talk(string message)
        {
            var ss = new SpeechSynthesizer();
            ss.Speak(message);
        }

        private bool _isLoaded
        {
            get { return _modules.Count > 0; }
        }

        #endregion

        #region Recognizer Events

        private void recognizer_SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
        }

        private void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            var m = e.Result.Text.ToLower();

            if (!_isLoaded && m.Contains(_computerName.ToLower()))
            {
                LoadAllModules();

                Talk("Yes " + _userName + "?");
                return;
            }

            if (_isLoaded && m.Contains(_stopListening))
            {
                UnloadAllModules();

                Talk(_stopListeningMessage);
                return;
            }

            foreach (var mod in _modules)
            {
                string s = mod.Match(m);
                if (!string.IsNullOrEmpty(s))
                {
                    Talk(s);
                    return;
                }
            }
        }

        #endregion
    }
}
