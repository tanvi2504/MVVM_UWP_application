/*
 * RelayCommand.cs
 * @Author : Tanvi Raut
 * Implements the Button Commands
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace T9_Project2.ViewModel
{
    /// <summary>
    /// Implements ICommand interface and the corresponding methods
    /// </summary>
    public class RelayCommand : ICommand
    {
        private MainViewModel viewModel;

        public RelayCommand(MainViewModel vm)
        {
            viewModel = vm;
        }

        /// <summary>
        ///Execute the button click
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns>TRUE</returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// calls the method in viewModel on button click
        /// </summary>
        /// <param name="parameter">Command parameter from Xaml</param>
        public void Execute(object parameter)
        {
            viewModel.displayWord(parameter.ToString());
        }

        public event EventHandler CanExecuteChanged;
    }
}
