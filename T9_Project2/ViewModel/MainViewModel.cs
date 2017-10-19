/*
 * MainViewModel.cs
 * @Author : Tanvi Raut
 * Implements the ViewModel for T9 Messenger
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using T9_Project2.Model;

namespace T9_Project2.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        T9Model model = new T9Model();
        private string nonPredictiveText = "";
        private string predictiveText = "";
        private bool isChecked;
        int next = 1;
        string keyPress = ""; //keeps the trackof key pressed for preditive
        string previousSentence = "";

        //list of predicted words returned from Model
        List<string> predictedWords;

        string[] currentWords;

        bool checkBoxChecked = false;
        //keeps track of the current word for backspace purpose
        string currentWord = "";

        //Property that binds with the view for checkbox
        public bool IsPredictive
        {
            get
            {
                return isChecked;
            }
            set
            {
                isChecked = value;
                //notify if the checkbox vlue is changed
                OnPropertyChanged("IsPredictive");
                if (checkBoxChecked != isChecked)
                {
                    NonPredictiveTextBox = "";
                    PredictiveTextBox = "";
                    keyPress = "";
                }
            }
        }

        //Property that binds with the view for preditive text in gray
        public string PredictiveTextBox
        {
            get
            {
                return predictiveText;
            }
            set
            {
                predictiveText = value;
                OnPropertyChanged("PredictiveTextBox");
            }
        }

        //Property that binds with the view for non-preditive text in black
        public string NonPredictiveTextBox
        {
            get
            {
                return nonPredictiveText;
            }
            set
            {
                nonPredictiveText = value;
                OnPropertyChanged("NonPredictiveTextBox");
            }
        }
        //binds the RelayCommnd with the viewModel
        private RelayCommand m_ButtonCommand;
        public ICommand ButtonCommand
        {
            get
            {
                return m_ButtonCommand;
            }

        }

        public MainViewModel()
        {
            m_ButtonCommand = new RelayCommand(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        //INotifyPropertyChanged method implemented
        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// This method is called from the Relay Command upon receiving the input 
        /// command parameter from view
        /// </summary>
        /// <param name="input">command parameter in string formt</param>
        public void displayWord(string input)
        {
            //Non-preditive mode
            if (IsPredictive == false)
            {
                //checkBoxChecked is true if non-predictive mode is on
                checkBoxChecked = false;
                DateTime current_time = DateTime.Now; //button click time
                current_time = current_time.ToUniversalTime();
                NonPredictiveTextBox = model.nonPredictiveMode(input, NonPredictiveTextBox, current_time);
            }
            else
            {
                //checkBoxChecked is true if predictive mode is on
                checkBoxChecked = true;
                //Next: circulates through the list
                if (input == "0")
                {
                    if (predictedWords.Count > 1)
                    {
                        processList(predictedWords, keyPress, next);
                        next++;
                    }
                }
                //delete button
                else if (input == "*")
                {
                    next = 1;
                    if (NonPredictiveTextBox.Length > 1)
                    {
                        //if the last letter is a space
                        if (NonPredictiveTextBox.ElementAt(NonPredictiveTextBox.Length - 1) == ' ')
                        {
                            //remove space
                            NonPredictiveTextBox = NonPredictiveTextBox.Remove(NonPredictiveTextBox.Length - 1);
                           
                            if (!(NonPredictiveTextBox.ElementAt(NonPredictiveTextBox.Length - 1) == ' '))
                            {
                                currentWords = NonPredictiveTextBox.Split(' ');
                                //keep track of last word
                                currentWord = currentWords.Last();
                                //remove last letter
                                previousSentence = NonPredictiveTextBox.Remove(NonPredictiveTextBox.Length - currentWord.Length);
                                NonPredictiveTextBox = "";
                                PredictiveTextBox = "";
                                //get the matching keys  for that word from model 
                                keyPress = model.getButtonNumber(currentWord);
                                //based on those keys, again obtain the list of predicted words
                                predictedWords = model.predictiveMode(keyPress);
                                int index = predictedWords.IndexOf(currentWord);
                                processList(predictedWords, keyPress, index);
                            }
                        }
                        //if previous letter is not space
                        else
                        {
                            NonPredictiveTextBox = NonPredictiveTextBox.Remove(NonPredictiveTextBox.Length - 1);
                            PredictiveTextBox = "";
                            if (keyPress != "")
                            {
                                keyPress = keyPress.Remove(keyPress.Length - 1);
                                predictedWords = model.predictiveMode(keyPress);
                                processList(predictedWords, keyPress, 0);
                            }
                        }
                    }
                    //if textbox is empty or has only one letter
                    else if (NonPredictiveTextBox.Length <= 1)
                    {
                        keyPress = "";
                        NonPredictiveTextBox = "";
                        PredictiveTextBox = "";
                        previousSentence = "";
                    }
                }
                //hit space and accept the word
                else if (input == "#")
                {
                    if (NonPredictiveTextBox.Contains("-"))
                    {
                        //if textblock contains hypens the do nothing
                    }
                    //else add space 
                    else
                    {
                        NonPredictiveTextBox += PredictiveTextBox + " ";
                        PredictiveTextBox = "";
                        previousSentence = NonPredictiveTextBox;
                        next = 1;
                        keyPress = "";
                    }
                }
                else
                {
                    //if textblock has vlid words, find the list of predicted words 
                    //for the input key press
                    if (!NonPredictiveTextBox.Contains("-"))
                    {
                        keyPress += input;
                        predictedWords = model.predictiveMode(keyPress);
                        processList(predictedWords, keyPress, 0);
                        next = 1;
                    }
                }
            }
        }

        /// <summary>
        /// obtain the first word in the list and display
        /// </summary>
        /// <param name="predictedWords">list of predicted words obtained from model</param>
        /// <param name="keyPress">the input buttons clicked</param>
        /// <param name="index">index to iterate through the predicted words list</param>
        public void processList(List<string> predictedWords, string keyPress, int index)
        {
            //if the word is invalid, display hypens
            if (predictedWords.Count == 0)
            {
                PredictiveTextBox = "";
                NonPredictiveTextBox = previousSentence;
                for (int i = 0; i < keyPress.Length; i++)
                {
                    NonPredictiveTextBox += "-";
                }
            }
            else
            {
                string displayWord = "";
                if (keyPress.Length > 0)
                {
                    displayWord = predictedWords.ElementAt(index);

                    //prefix word based on key press
                    if (keyPress.Length == displayWord.Length)
                    {
                        NonPredictiveTextBox = previousSentence + displayWord;
                        PredictiveTextBox = "";
                    }
                    //predicted words
                    else
                    {
                        NonPredictiveTextBox = previousSentence + displayWord.Substring(0, keyPress.Length);
                        PredictiveTextBox = displayWord.Substring(keyPress.Length);
                    }
                }
            }
        }
    }
}