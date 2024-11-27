using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Wordle
{
    public partial class MainWindow : Window
    {
        private string secretWord = "HELLO"; //Hardcode hemmeligt ord for at validere at programmet virker.
        private int currentRow = 0; //Holder styr på hvilken rækker brugeren er på.
        private int currentColumn = 0; //Holder styr på hvilken kolonne brugeren er på.

        private TextBox[,] textBoxes;
        public MainWindow()
        {
            InitializeComponent();
            this.PreviewKeyDown += HandleKeyPress; //Opfanger hvad der bliver tastet på tastaturet.

            // Initialisere hardcoded TextBox referencer
            textBoxes = new TextBox[,] //2D array der lagre referencer til TextBox elementerne i programmets UI. Hver 'TextBox" representerer et bogstav i spillet.
            {
                { Textinput0_0, Textinput0_1, Textinput0_2, Textinput0_3, Textinput0_4 },
                { Textinput1_0, Textinput1_1, Textinput1_2, Textinput1_3, Textinput1_4 },
                { Textinput2_0, Textinput2_1, Textinput2_2, Textinput2_3, Textinput2_4 },
                { Textinput3_0, Textinput3_1, Textinput3_2, Textinput3_3, Textinput3_4 },
                { Textinput4_0, Textinput4_1, Textinput4_2, Textinput4_3, Textinput4_4 },
                { Textinput5_0, Textinput5_1, Textinput5_2, Textinput5_3, Textinput5_4 }
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e) //Metoden kaldes hvis en knap trykkes på i programmets UI. Tak intellisense :D
        {
            if (sender is Button button) // Hvis "sender" (en knap) er en knap, valideres inputtet og instansen af knappen tildeles til variablen "button"
            {
                string buttonContent = button.Content.ToString();

                if (buttonContent == "Enter") //Håndterer når "Enter"-knappen trykkes, hvilket får programmet til at evaluere indputtet sammenholdt med det rigtige ord.
                {
                    ValidateGuess();
                }
                else if (buttonContent == "Delete" || buttonContent == "Backsp") //Håndterer "Delete"/"Backspace"-knapperne som fjerner et bogstav.
                {
                    RemoveLetter();
                }
                else if (buttonContent.Length == 1 && char.IsLetter(buttonContent[0])) //Håndterer knapperne til almindelige bogstaver. 'Length == 1' sørger for at at knappen har præcis et tegn. 'char.IsLetter' sørger for at tal og specialbogstaver ikke kan tastes. '[0]' undersøger om det første tegn i knappen er et bogstav.
                {
                    AddLetter(buttonContent.ToUpper()); //Konverterer indput fra knapper til almindelige bogstaver og gør dem "store" uanset hvad.
                }
            }
        }

        private void HandleKeyPress(object sender, KeyEventArgs e) //Undersøger om knappen der trykkes på holder sig inden for de 26 bogstaver i det engelske alfabet sammen med koden ovenover.
        {
            if (e.Key >= Key.A && e.Key <= Key.Z && currentColumn < 5) //Der kan tastes alt fra A til Z og så længe brugeren er inden for de 5 kasser i en række.
            {
                AddLetter(e.Key.ToString().ToUpper());
            }
            else if (e.Key == Key.Back) //Sletter det seneste bogstav, se længere nede
            {
                RemoveLetter();
            }
            else if (e.Key == Key.Enter) //Evaluere om gættet er korrekt. se længere nede.
            {
                ValidateGuess();
            }
        }

        private void AddLetter(string letter)  //Tilføjer et bogstav.
        {
            if (currentColumn < 5) //Hvis brugeren er i et felt der svarer til index 0 til 4, så
            {
                textBoxes[currentRow, currentColumn].Text = letter; //tilføjes et bogstav som svarer til hvad AddLetter metoden evaluerer.
                currentColumn++; //Herefter rykker programmet 1 plads frem i kolonnerne; altså én plads til højre. 
            }
        }

        private void RemoveLetter() //Sletter et bogstav
        {
            if (currentColumn > 0) //Hvis brugeren befinder sig i et felt der svarer til index 1 til 4, så
            {
                currentColumn--; //rykker programmet én plads tilbage (til venstre) og
                textBoxes[currentRow, currentColumn].Text = ""; //efterlader feltet programmet gik fra med en tom streng.
            }
        }

        private void ValidateGuess() //Metoden der evaluere om brugerens indtasting svarer til hvad programmet anser som det rigtige ord (secretWord)..
        {
            if (currentColumn < 5) //Hvis brugeren er i index 0 til og med 4, men der ikke er tastet noget ind i index 4, så
            {
                MessageBox.Show("Tast alle 5 bogstaver inden der trykkes Enter."); //vises en 'fejl'-meddelelse. 
                return;
            }

            bool isCorrect = true; //Hjæper med at holde styr på om gættet er korrekt.

            for (int i = 0; i < 5; i++) //Validerer hvert bogstav i ordet.
            {
                string guessLetter = textBoxes[currentRow, i].Text.ToUpper(); //Tager bogstavet i (currentRow, i) og konvertere det til "store" bogstaver, hvilket sørger for at sammenligningen ikke fejler grundet små bogstaver tidligere i koden fx. fra ordlisten eller hvad end der er hardcoded som det rigtige ord.
                if (guessLetter == secretWord[i].ToString()) //Hvis bogstavet i gættet svarer til det respektive bogstav i det hemmelige ord i samme position (==), så
                {
                    textBoxes[currentRow, i].Background = new SolidColorBrush(Colors.Green); //valideres bogstavet med en grøn farve da betingelsen i if sætningen er sand.
                }
                else if (secretWord.Contains(guessLetter)) //Eller hvis bogstavet findes i ordet (Contains(guessletter)),
                {
                    textBoxes[currentRow, i].Background = new SolidColorBrush(Colors.Yellow); //bliver farven gul.
                    isCorrect = false;//False fordi betingelsen ikke er opfyldt. Vi oprerere kun med sandt og falsk (boolean) og hvis ikke det er sandt, så er det falsk, men vi har formateret den falske betingelse her på en måde så den visuelt anses som at være delvist rigtig selvom programmet anser det for at være lige så falsk som else sætningen.
                }
                else //Ellers
                {
                    textBoxes[currentRow, i].Background = new SolidColorBrush(Colors.Gray); //bliver farven grå.
                    isCorrect = false; 
                }
            }

            if (isCorrect) //Efter programmet evaluere alle bogstaverne i en række, hvis isCorrect stadig er sand/true, så
            {
                MessageBox.Show("Tillykke!! Du gættede ordet! :D"); //så vises en sejrsmeddelelse og
                ResetGame(); //spillet nulstilles.
                return; //Return sørger for at der ikke køres mere kode.
            }

            currentRow++; //Rykker ned til næste række hvis is´Correct evalueres til falsk.
            currentColumn = 0; //Starter næste række ved index 0.

            if (currentRow >= 6) //Hvis currentRow er større eller lig med 6 (7'ende linje da det svarer til index 6), så
            {
                MessageBox.Show($"Game over! Det rigtige ord var {secretWord}."); //vises en nederlagsmeddelelse og
                ResetGame(); //spillet nulstilles.
            }
        }

        private void ResetGame() //Metode til at nulstille spillet, som kaldes hvis spillet vindes eller man taber som vist ovenover.
        {
            foreach (var textBox in textBoxes) //Interere for hvert element i programmets array i starten af koden og
            {
                textBox.Text = ""; //nulstiller alle 'kasser' med en tom streng itakt med programmet iterere.
                textBox.Background = new SolidColorBrush(Colors.White); //Baggrundsfarven overskrives med farven hvid, da dette er farven der er valgt som 'start-farve'/tom-kasse-farven.
            }

            currentRow = 0; //Programmet går tilbage til index 0 i rækkerne.
            currentColumn = 0; //Programmet går tilbage til index 0 i kolonnerne.
        }
    }
}