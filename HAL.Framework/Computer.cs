using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using HAL.Framework.Extensions;
using HAL.Framework.Modules;

namespace HAL.Framework
{
    public class Computer
    {
        #region Private Properties

        private readonly SpeechRecognitionEngine _recognizer;

        private List<IModule> _lockedModules { get; set; }
        private List<IModule> _modules { get; set; }
        private List<IModule> _possibleModules { get; set; }

        private IModule _startListeningModule
        {
            get
            {
                return _lockedModules.FirstOrDefault(x => x is StartListeningModule);
            }
        }
        private IModule _stopListeningModule
        {
            get
            {
                return _lockedModules.FirstOrDefault(x => x is StopListeningModule);
            }
        }

        #endregion

        #region Constructor

        public Computer()
        {
            #region Initialize Variables

            _lockedModules = new List<IModule>();
            _modules = new List<IModule>();
            _possibleModules = new List<IModule>();

            #endregion

            #region Setup Recognizer

            _recognizer = new SpeechRecognitionEngine();
            _recognizer.SetInputToDefaultAudioDevice();

            _recognizer.SpeechRecognized += recognizer_SpeechRecognized;
            _recognizer.SpeechHypothesized += recognizer_SpeechHypothesized;

            #endregion

            #region Setup Locked Modules

            _lockedModules.Add<StartListeningModule>();
            _lockedModules.Add<StopListeningModule>();

            foreach (var l in _lockedModules)
                _recognizer.LoadGrammar(l.Grammar);

            _recognizer.RecognizeAsync(RecognizeMode.Multiple);

            #endregion
        }

        #endregion

        #region Module Support

        public bool LoadModule<T>() where T : IModule, new()
        {
            IModule m = ModuleFactory.CreateModule<T>();
            return LoadModule(m);
        }

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

        public void FeedInput(string input)
        {
            _recognizer.EmulateRecognize(input);
        }

        #endregion

        #region Recognizer Events

        private void recognizer_SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
        }

        private void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            var m = e.Result.Text.ToLower();

            #region Start Listening

            if (e.Result.Grammar == _startListeningModule.Grammar)
            {
                if (!_isLoaded)
                {
                    var s = _startListeningModule.Match(m, LoadAllModules);

                    if (s.IsNotEmpty())
                        Talk(s);
                }
            }

            #endregion

            #region Stop Listening

            else if (e.Result.Grammar == _stopListeningModule.Grammar)
            {
                if (_isLoaded)
                {
                    var s = _stopListeningModule.Match(m, UnloadAllModules);

                    if (s.IsNotEmpty())
                        Talk(s);
                }
            }

            #endregion

            #region Check Modules

            else
            {
                var mod = _modules.FirstOrDefault(x => x.Grammar == e.Result.Grammar);
                string response = mod != null ? mod.Match(m) : string.Empty;

                if (response.IsNotEmpty())
                    Talk(response);
                else
                    Talk("I'm sorry, can you try saying that again?");
            }

            #endregion
        }

        #endregion
    }
}
