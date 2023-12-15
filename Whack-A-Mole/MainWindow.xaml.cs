using NAudio.Wave;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace WhackAMole
{
    public class LoopStream : WaveStream
    {
        private WaveStream sourceStream;

        public LoopStream(WaveStream sourceStream)
        {
            this.sourceStream = sourceStream;
            this.EnableLooping = true;
        }

        public bool EnableLooping { get; set; }

        public override WaveFormat WaveFormat
        {
            get { return sourceStream.WaveFormat; }
        }

        public override long Length
        {
            get { return sourceStream.Length; }
        }

        public override long Position
        {
            get { return sourceStream.Position; }
            set { sourceStream.Position = value; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int totalBytesRead = 0;
            while (totalBytesRead < count)
            {
                int bytesRead = sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
                if (bytesRead == 0)
                {
                    if (sourceStream.Position == 0 || !EnableLooping)
                    {
                        // End of the stream
                        break;
                    }
                    // Loop
                    sourceStream.Position = 0;
                }
                totalBytesRead += bytesRead;
            }
            return totalBytesRead;
        }
    } // Loopstream for the background music

    public partial class MainWindow : Window
    {

        #region Initialization and Setup

        /// <summary>
        /// Initialize game components, including UI elements, sound effects, and background music.
        /// </summary>

        private List<Button> moleButtons; // List to store mole buttons
        private Random rnd = new Random(); // Random number generator
        private DispatcherTimer gameTimer; // Timer for the game's duration
        private DispatcherTimer moleTimer; // Timer to control mole appearances
        private Dictionary<Button, DispatcherTimer> hideMoleTimers = new Dictionary<Button, DispatcherTimer>();
        private MediaPlayer backgroundMusicPlayer = new MediaPlayer(); // Separate player for background music
        private AudioFileReader moleSoundReader;
        private AudioFileReader catSoundReader;
        private AudioFileReader goldSoundReader;
        private AudioFileReader thudSoundReader;
        private IWavePlayer soundEffectPlayer;
        private IWavePlayer waveOut;
        private AudioFileReader audioFileReader;
        private bool hitProcessed = false;
        private DispatcherTimer hitDebounceTimer;


        public MainWindow()
        {
            InitializeComponent(); // Initialize the window components
            InitializeMoleButtons(); // Set up the mole buttons
            InitializeBackgroundMusic(); // set up background music
            InitializeSoundEffects(); // set up sound effects for mole, cat, and gold

            BGMSlider.Value = 50; // This will trigger the ValueChanged event
            SFXSlider.Value = 50; // This will trigger the ValueChanged event
            hitDebounceTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            hitDebounceTimer.Tick += (sender, e) => {
                hitProcessed = false;
                hitDebounceTimer.Stop();
            };

        }

        private void InitializeMoleButtons()
        {
            moleButtons = new List<Button>()
    {
        Mole1, Mole2, Mole3, Mole4, Mole5, Mole6, Mole7, Mole8, Mole9
    };

            foreach (var moleButton in moleButtons)
            {
                var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
                timer.Tick += (sender, e) =>
                {
                    ((DispatcherTimer)sender).Stop();
                    moleDisplay(moleButton, false); // Hide the mole
                };
                hideMoleTimers[moleButton] = timer;
            }
        }

        private void InitializeBackgroundMusic()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string backgroundMusicPath = Path.Combine(basePath, "Sounds", "BackgroundMusic.mp3");

            audioFileReader = new AudioFileReader(backgroundMusicPath);
            LoopStream loop = new LoopStream(audioFileReader);
            waveOut = new WaveOutEvent();
            waveOut.Init(loop);
            waveOut.Play();
        }

        private void InitializeSoundEffects()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            moleSoundReader = new AudioFileReader(Path.Combine(basePath, "Sounds", "Mole.mp3"));
            catSoundReader = new AudioFileReader(Path.Combine(basePath, "Sounds", "Cat1.mp3"));
            goldSoundReader = new AudioFileReader(Path.Combine(basePath, "Sounds", "Gold1.mp3"));
            thudSoundReader = new AudioFileReader(Path.Combine(basePath, "Sounds", "Thud.mp3"));

            soundEffectPlayer = new WaveOutEvent();
        }

        #endregion

        #region UI Event Handlers

        /// <summary>
        /// Handle user interactions with the game's UI elements.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void GameSettings(object sender, RoutedEventArgs e)
        {
            StartupMenu.Visibility = Visibility.Hidden;
            SettingsMenu.Visibility = Visibility.Visible;
        }

        private void ReturnToMenu(object sender, RoutedEventArgs e)
        {
            bool isValidInput = true;
            GameOverPanel.Visibility = Visibility.Hidden;
            // Validate Game Time
            if (int.TryParse(GameTimeValue.Text, out int userTime) && userTime > 0)
            {
                gameTime = userTime; // Update the game time
                startTime = userTime;
            }
            else
            {
                MessageBox.Show("Invalid input. Please enter a positive number for the game time.");
                isValidInput = false;
            }

            // Validate Hit Points
            if (int.TryParse(UserHitPoints.Text, out int userHitPointsValue) && userHitPointsValue > 0)
            {
                HitPoints = userHitPointsValue;
                HitPointsActive = userHitPointsValue;
            }
            else
            {
                MessageBox.Show("Invalid input. Please enter a positive number for the Health.");
                isValidInput = false;
            }

            // Only return to the start menu if both inputs are valid
            if (isValidInput)
            {
                SettingsMenu.Visibility = Visibility.Hidden; // Hide the settings menu
                StartupMenu.Visibility = Visibility.Visible; // Show the startup menu
            }
        }


        private void BGMSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (waveOut != null)
            {
                waveOut.Volume = (float)BGMSlider.Value / 100;
            }
        }

        private void SFXSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            float sfxVolume = (float)SFXSlider.Value / 100;
            if (moleSoundReader != null)
            {
                moleSoundReader.Volume = sfxVolume;
            }
            if (catSoundReader != null)
            {
                catSoundReader.Volume = sfxVolume;
            }
            if (goldSoundReader != null)
            {
                goldSoundReader.Volume = sfxVolume;
            }
            if (thudSoundReader != null)
            {
                thudSoundReader.Volume = sfxVolume;
            }
        }

        #endregion

        #region Game Mechanics

        /// <summary>
        /// Core game logic including starting, playing, 
        /// and ending the game, as well as handling game timers and random events.
        /// </summary>

        private int score = 0; // Player's score
        private int amount = 0; // variable for score increase
        private int gameTime = 300; // Time remaining in the game
        private int startTime; // Time set at beginning
        private int HighScore = 0; // High Score
        private int HitPoints = 3; // Health
        private int HitPointsActive;
        double chanceTwo = 0;
        double chanceThree = 0;
        int timeElapsedTwo = 0;
        int timeElapsedThree = 0;

        private void StartGame(object sender, RoutedEventArgs e)
        {
            score = 0; // Reset the score
            SetupGameTimer(); // Set up the game timer
            SetupMoleTimer(); // Set up the mole appearance timer
            HitPointsActive = HitPoints;
            ScoreTextBlock.Text = score.ToString(); // Display the score
            StartupMenu.Visibility = Visibility.Hidden;
            HitPointsValue.Text = HitPointsActive.ToString();
        }

        private void SetupGameTimer()
        {
            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromSeconds(1); // Set interval to 1 second
            gameTimer.Tick += GameTimer_Tick; // Subscribe to Tick event
            gameTimer.Start(); // Start the timer
        }

        private void SetupMoleTimer()
        {
            moleTimer = new DispatcherTimer();
            moleTimer.Tick += MoleTimer_Tick;
            moleTimer.Interval = TimeSpan.FromSeconds(GetRandomDouble()); // Initial interval
            moleTimer.Start();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            gameTime--; // Decrease remaining time
            TimeTextBlock.Text = gameTime.ToString(); // Update time display

            if (gameTime <= 0) // Check if time has run out
            {
                gameTimer.Stop(); // Stop the timer
                EndGame(); // End the game
            }
        }

        private void MoleTimer_Tick(object sender, EventArgs e)
        {
            showRandomMole();
            moleTimer.Interval = TimeSpan.FromSeconds(GetRandomDouble()); // Adjust interval
        }

        private void showRandomMole()
        {
            if (Difficulty.SelectedIndex == 0)
            {
                chanceTwo = 0.3;
                timeElapsedTwo = 30;
                chanceThree = 0.2;
                timeElapsedThree = 60;
            }
            else if (Difficulty.SelectedIndex == 1)
            {
                chanceTwo = 0.4;
                timeElapsedTwo = 20;
                chanceThree = 0.3;
                timeElapsedThree = 50;
            }
            else
            {
                chanceTwo = 0.4;
                timeElapsedTwo = 15;
                chanceThree = 0.4;
                timeElapsedThree = 25;
            }

            // Clear existing moles
            foreach (var moleButton in moleButtons)
            {
                moleDisplay(moleButton, false);
            }
             // Calculate elapsed time as a percentage
            double elapsedTimePercentage = 1 - ((double)gameTime / startTime);
             // Set chances for the number of moles to show
            int chanceForTwoMoles = elapsedTimePercentage > chanceTwo ? timeElapsedTwo : 0; // 30% chance after 30% time elapsed
            int chanceForThreeMoles = elapsedTimePercentage > chanceThree ? timeElapsedThree : 0; // Additional 20% chance after 60% time elapsed
             // Determine the number of moles to show based on random chance
            int randomNumber = rnd.Next(100);
            int molesToShow = 1; // Default is 1 mole
            if (randomNumber < chanceForThreeMoles)
            {
                molesToShow = 3;
            }
            else if (randomNumber < chanceForTwoMoles + chanceForThreeMoles)
            {
                molesToShow = 2;
            }
             // Show moles
            HashSet<int> displayedMoles = new HashSet<int>();
            while (displayedMoles.Count < molesToShow)
            {
                int index = rnd.Next(moleButtons.Count);
                if (!displayedMoles.Contains(index))
                {
                    moleDisplay(moleButtons[index], true);
                    displayedMoles.Add(index);
                }
            }
        }

        private void moleClick(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                // Check if the clicked button has a creature
                if (clickedButton.Tag != null)
                {
                    string tag = clickedButton.Tag.ToString();
                    switch (tag)
                    {
                        case "Mole":
                            amount = 1;
                            points(amount);
                            break;
                        case "GoldMole":
                            amount = 3;
                            points(amount);
                            break;
                        case "Friend":
                            amount = -1;
                            points(amount);
                            break;
                    }
                    moleDisplay(clickedButton, false); // Hide the creature immediately
                    hideMoleTimers[clickedButton]?.Stop(); // Stop the timer for this button
                    clickedButton.IsEnabled = true; // Re-enable the button
                }
                else
                {
                    amount = 0;
                    points(amount);
                }
            }
        }

        private void points(int amount)
        {
            score += amount;
            ScoreTextBlock.Text = score.ToString();

            if (soundEffectPlayer.PlaybackState == PlaybackState.Playing)
            {
                soundEffectPlayer.Stop();
            }

            AudioFileReader selectedSound = null;

            switch (amount)
            {
                case 1:
                    selectedSound = moleSoundReader;
                    break;
                case 3:
                    selectedSound = goldSoundReader;
                    break;
                case -1:
                    selectedSound = catSoundReader;
                    Hit();
                    Lives();
                    break;
                case 0:
                    selectedSound = thudSoundReader;
                    Hit();
                    Lives();
                    break;
            }
            

            if (selectedSound != null)
            {
                selectedSound.Position = 0; // Reset to the start
                soundEffectPlayer.Init(selectedSound);
                soundEffectPlayer.Play();
            }
        }

        private async void Hit()
        {
            if (hitProcessed) return;

            hitProcessed = true;
            HitPanel.Visibility = Visibility.Visible;
            await Task.Delay(100);
            HitPanel.Visibility = Visibility.Collapsed;

            hitDebounceTimer.Start();
        }


        private int Lives()
        {
            if (HitPointsActive > 0)
            {
                HitPointsActive--;
            }

            if (HitPointsActive <= 0)
            {
                EndGame();
            }

            HitPointsValue.Text = HitPointsActive.ToString();
            return HitPointsActive;
        }


        private void EndGame()
        {
            
            if (score > 0 && score < HighScore)
            {
                HighScoreText.Text = $"High Score: {HighScore}";
            }
            else if (score > 0 && score > HighScore)
            {
                HighScore = score;
                HighScoreText.Text = $"New High Score! {HighScore}";
            }
            else HighScoreText.Text = $"High Score: {HighScore}";
            gameTimer?.Stop(); // Stop the game timer
            moleTimer?.Stop(); // Stop the mole timer

            // Determine the reason for game over
            string gameOverReason = gameTime <= 0 ? "Time's up!" : "All lives lost!";
            GameOverText.Text = "Game Over"; // Set game over text
            FinalScoreText.Text = $"Final Score: {score}\n{gameOverReason}"; // Show final score and reason

            GameOverPanel.Visibility = Visibility.Visible; // Show the game over panel
            StartButton.IsEnabled = true;
            TryAgainButton.IsEnabled = true;
        }

        private double GetRandomDouble()
        {
            double slowest = 2;
            double timeFactor = (double)gameTime / startTime;
            if (Difficulty.SelectedIndex == 0)
            {
                slowest = timeFactor*4 > 1 ? 2 : 1; // Example scaling
            }
            else if (Difficulty.SelectedIndex == 1)
            {
                slowest = timeFactor*3 > 1 ? 2 : 1; // Example scaling
            }
            else if (Difficulty.SelectedIndex == 2)
            {
                slowest = timeFactor*2 > 1 ? 2 : 1; // Example scaling
            }
            // Adjust the frequency based on remaining time
            double fastest = 1;
            double randomValue = rnd.NextDouble() * (slowest - fastest) + fastest;
            return randomValue;
        }

        private int PercentForAppearance()
        {
            int result;
            int randPercent = rnd.Next(1, 101);
            if (randPercent >= 1 && randPercent <= 85) result = 1; // 85 percent chance for regular mole 
            else if (randPercent > 85 && randPercent <= 95) result = 2; // 10 percent chance for friendly
            else result = 3; // 5 percent chance for golden mole
            return result;
        }
        

        #endregion

        #region UI Update and Animation

        /// <summary>
        /// Methods for updating the UI and handling animations,
        /// such as mole appearances and movements.
        /// </summary>
        /// <param name="moleButton"></param>
        /// <param name="showMole"></param>

        private void moleDisplay(Button moleButton, bool showMole)
        {
            moleButton.ClipToBounds = true;

            if (showMole)
            {
                // Create a new Image for the creature and apply a TranslateTransform
                Image creatureImage = new Image();
                TranslateTransform moveTransform = new TranslateTransform();
                creatureImage.RenderTransform = moveTransform;

                // Set the image source based on the type of creature
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string imagePath = ""; // Initialize with an empty string
                int rand = PercentForAppearance();
                switch (rand)
                {
                    case 1:
                        imagePath = Path.Combine(baseDirectory, "Sprites", "Mole.png");
                        moleButton.Tag = "Mole";
                        break;
                    case 2:
                        imagePath = Path.Combine(baseDirectory, "Sprites", "GoldMole.png");
                        moleButton.Tag = "GoldMole";
                        break;
                    case 3:
                        imagePath = Path.Combine(baseDirectory, "Sprites", "Cat.png");
                        moleButton.Tag = "Friend";
                        break;
                    default:
                        return; // If no valid type is found, exit the method
                }

                creatureImage.Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
                moleButton.Content = creatureImage;

                // Slide the creature image into view
                SlideIn(creatureImage);

                // Reset and start the timer for automatically hiding the creature
                hideMoleTimers[moleButton].Stop();
                hideMoleTimers[moleButton].Start();
            }
            else
            {
                // Slide the creature image out of view
                Image creatureImage = moleButton.Content as Image;
                if (creatureImage != null)
                {
                    SlideOut(creatureImage);

                    // Use a timer to clear the content of the button after the animation completes
                    var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(0.08) };
                    timer.Tick += (sender, e) =>
                    {
                        ((DispatcherTimer)sender).Stop();
                        moleButton.Content = null; // Clear the button content
                        moleButton.Tag = null; // Clear the tag
                    };
                    timer.Start();
                }
            }
        }

        private bool AreAnyMolesDisplayed()
        {
            return moleButtons.Any(button => button.Content != null);
        }

        private void SlideIn(Image creatureImage)
        {
            DoubleAnimation slideInAnimation = new DoubleAnimation
            {
                From = 100, // Start from below the button (adjust this value as needed)
                To = 0,
                Duration = TimeSpan.FromSeconds(0.08)
            };

            TranslateTransform transform = creatureImage.RenderTransform as TranslateTransform;
            transform?.BeginAnimation(TranslateTransform.YProperty, slideInAnimation);
        }

        private void SlideOut(Image creatureImage)
        {
            DoubleAnimation slideOutAnimation = new DoubleAnimation
            {
                From = 0,
                To = 100, // Move below the button (adjust this value as needed)
                Duration = TimeSpan.FromSeconds(0.08)
            };

            TranslateTransform transform = creatureImage.RenderTransform as TranslateTransform;
            transform?.BeginAnimation(TranslateTransform.YProperty, slideOutAnimation);
        }


        #endregion

        #region Cleanup and Disposal

        /// <summary>
        /// Handle the cleanup and disposal of resources when the game is closed.
        /// </summary>
        /// <param name="e"></param>

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (waveOut != null)
            {
                waveOut.Stop();
                waveOut.Dispose();
            }

            if (soundEffectPlayer != null)
            {
                soundEffectPlayer.Dispose();
            }

            if (moleSoundReader != null)
            {
                moleSoundReader.Dispose();
            }

            if (catSoundReader != null)
            {
                catSoundReader.Dispose();
            }

            if (goldSoundReader != null)
            {
                goldSoundReader.Dispose();
            }

            if (thudSoundReader != null)
            {
                thudSoundReader.Dispose();
            }

            // For MediaPlayer, just stop the playback
            if (backgroundMusicPlayer != null)
            {
                backgroundMusicPlayer.Stop();
            }
        }

        #endregion

    }
}
