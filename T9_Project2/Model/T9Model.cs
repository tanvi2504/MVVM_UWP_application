/*
 * T9Model.cs
 * @Author : Tanvi Raut
 * Implements Model for T9 Messenger
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T9_Project2.Model
{
    public class T9Model
    {
        char[][] keypad;     
        //UnixEpoch is used to calculate the epoch difference between button clicks
        private static DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime previousTime = UnixEpoch;

        private int clicksCount = 0;

        private string lastInput = "";
        Dictionary<string, string> wordToKey;
        //converts the input word list to lowercase
        string lowerCase="";

        /// <summary>
        /// This constructor reds the input file for the list of dictionary words
        /// and maps the words to their corresponding numbers
        /// </summary>
        public T9Model()
        {
            //used for non-pedictive keypress
            keypad = new char[11][];
            keypad[0] = new char[] { '1' };
            keypad[1] = new char[] { 'a', 'b', 'c', '2' };
            keypad[2] = new char[] { 'd', 'e', 'f', '3' };
            keypad[3] = new char[] { 'g', 'h', 'i', '4' };
            keypad[4] = new char[] { 'j', 'k', 'l', '5' };
            keypad[5] = new char[] { 'm', 'n', 'o', '6' };
            keypad[6] = new char[] { 'p', 'q', 'r', 's', '7' };
            keypad[7] = new char[] { 't', 'u', 'v', '8' };
            keypad[8] = new char[] { 'w', 'x', 'y', 'z', '9' };
            keypad[9] = new char[] { '0', '~' };
            keypad[10] = new char[] { ' ' };

            // Reads the file
            string[] words = File.ReadAllLines("english-words.txt");            
            wordToKey = new Dictionary<string, string>();

            //map the words to the corresponding numbers and store in dictionary
            foreach (string word in words)
            {
                lowerCase = word.ToLower();
                string mappingKey = "";
                for (int i = 0; i < lowerCase.Length; i++)
                {
                    if (lowerCase[i] == 'a' || lowerCase[i]=='b' || lowerCase[i]=='c')
                    {
                        mappingKey += "2";
                    }
                    if (lowerCase[i] == 'd' || lowerCase[i] == 'e' || lowerCase[i] == 'f')
                    {
                        mappingKey += "3";
                    }
                    if (lowerCase[i] == 'g' || lowerCase[i] == 'h' || lowerCase[i] == 'i')
                    {
                        mappingKey += "4";
                    }
                    if (lowerCase[i] == 'j' || lowerCase[i] == 'k' || lowerCase[i] == 'l')
                    {
                        mappingKey += "5";
                    }
                    if (lowerCase[i] == 'm' || lowerCase[i] == 'n' || lowerCase[i] == 'o')
                    {
                        mappingKey += "6";
                    }
                    if (lowerCase[i] == 'p' || lowerCase[i] == 'q' || lowerCase[i] == 'r' || lowerCase[i]=='s')
                    {
                        mappingKey += "7";
                    }
                    if (lowerCase[i] == 't' || lowerCase[i] == 'u' || lowerCase[i] == 'v')
                    {
                        mappingKey += "8";
                    }
                    if (lowerCase[i] == 'w' || lowerCase[i] == 'x' || lowerCase[i] == 'y' || lowerCase[i]=='z')
                    {
                        mappingKey += "9";
                    }
                }
                wordToKey.Add(lowerCase, mappingKey);
            }
        }
        
        /// <summary>
        /// This methods generates the list of predicted words for the Prediction mode
        /// </summary>
        /// <param name="buttonNumberKey"> input button press</param>
        /// <returns>list of predicted words starting from the keys pressed</returns>
        public List<string> predictiveMode(string buttonNumberKey)
        {
            //using LINQ query
            List<string> predictedWordsList = wordToKey.Where(p => p.Value.StartsWith(buttonNumberKey)).Select(p => p.Key).ToList();
            predictedWordsList = predictedWordsList.OrderBy(r => r.Length).ThenBy(r => r).ToList();
            return predictedWordsList;
        }

        /// <summary>
        /// button numbers for corresponding words
        /// </summary>
        /// <param name="word">Input word</param>
        /// <returns>button number</returns>
        public string getButtonNumber(string word)
        {
            string number = wordToKey[word];
            return number;
        }

        /// <summary>
        /// This method is invoked for the non predictive mode
        /// Determines the input and returns the corresponding action for the textblock
        /// lso for mutiple clicks on single button, calculates the time difference
        /// </summary>
        /// <param name="input">key pressed</param>
        /// <param name="currentText">current word in text box</param>
        /// <param name="currentTime">time the button was clicked</param>
        /// <returns>the text for the textblock</returns>
        public string nonPredictiveMode(string input, string currentText, DateTime currentTime)
        {
            //if the same button was not hit
            if (input != lastInput)
            {
                clicksCount = 0;
            }
            lastInput = input;
            string nonPredictiveText = "";
            if (input == "1" || input == "0")
            {
                nonPredictiveText = currentText+ input;
            }
            //space
            else if (input == "#")
            {
                nonPredictiveText =currentText+ " ";
            }
            //delete
            else if (input == "*")
            {
                if (currentText.Length > 0)
                {
                    nonPredictiveText = currentText.Remove(currentText.Length - 1);
                }
            }
            else
            {
                int inputNum = Int32.Parse(input);
                //calculates time difference between the button clicked
                int millisceonds = (int)(currentTime - previousTime).TotalMilliseconds;
                bool checkIfMoreThanSec = millisceonds > 1000;

                //typing next different or same letter
                if (checkIfMoreThanSec || previousTime == UnixEpoch)
                {
                    clicksCount=0;
                    nonPredictiveText = currentText + keypad[inputNum-1].ElementAt(clicksCount);
                }

                //different letter but within 1 second
                else if (!checkIfMoreThanSec && clicksCount==0)
                {
                    nonPredictiveText = currentText + keypad[inputNum-1].ElementAt(clicksCount);                  
                }
                else
                {
                    if (clicksCount == keypad[inputNum - 1].Length)
                        clicksCount = 0;
                    nonPredictiveText = currentText.Remove(currentText.Length - 1) + keypad[inputNum-1].ElementAt(clicksCount);
                }
                clicksCount++;
            }
            previousTime = currentTime;
            return nonPredictiveText;
        }
    }
}
