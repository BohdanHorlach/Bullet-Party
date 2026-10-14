using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;


namespace UI.Input
{
    public class InputFieldValidator : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textField;
        [SerializeField] private int _requiredLength = 6;
        [SerializeField] private bool _upperCaseOutput = true;


        private string CleanupInput(string input)
        {
            return Regex.Replace(input.Trim(), @"[^\w\d]", "");
        }


        private bool IsValid(string input)
        {
            bool isValidLength = input.Length == _requiredLength;
            bool withoutSpecial = input.All(char.IsLetterOrDigit);

            return withoutSpecial && isValidLength;
        }


        public bool TryGetInput(out string output)
        {
            string input = CleanupInput(_textField.text);

            if (IsValid(input))
            {
                output = _upperCaseOutput ? input.ToUpper() : input;
                return true;
            }

            output = string.Empty;
            return false;
        }
    }
}