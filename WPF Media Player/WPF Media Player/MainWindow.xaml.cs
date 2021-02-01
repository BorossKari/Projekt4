using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;

namespace WPF_Media_Player
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int playing = 0;
        int slideruse = 0;
        DispatcherTimer timer = new DispatcherTimer();
        MediaPlayer mediaPlayer = new MediaPlayer();
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Opensongs_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MP3 files (*.mp3)|*.mp3|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
                mediaPlayer.Open(new Uri(openFileDialog.FileName));
            mediaPlayer.Play();
            playing = 1;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += TimerRefresher;
            timer.Start();
            TimeSlider.IsEnabled = true;
        }
        private void Removesong_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Saveplaylist_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Loadplaylist_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (playing == 0)
            {
                mediaPlayer.Play();
                playing = 1;
            }
            else if (playing == 1)
            {
                mediaPlayer.Pause();
                playing = 0;
            }timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += TimerRefresher;
            timer.Start();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
            playing = 0;
            TimeLabel.Content = "00:00";
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RepeatButton_Click(object sender, RoutedEventArgs e)
        {

        }
        void TimerRefresher(object sender, EventArgs e)
        {
            if (mediaPlayer.Source != null && (mediaPlayer.NaturalDuration.HasTimeSpan) && slideruse == 0)
            {
                TimeSlider.Minimum = 0;
                TimeSlider.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                TimeSlider.Value = mediaPlayer.Position.TotalSeconds;
            }
        }
        private void TimeSlider_DragStarted(object sender, DragStartedEventArgs e)
        {
            slideruse = 1;
        }

        private void TimeSlider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            slideruse = 0;
            mediaPlayer.Position = TimeSpan.FromSeconds(TimeSlider.Value);
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += TimerRefresher;
            timer.Start();
        }

        private void TimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TimeLabel.Content = TimeSpan.FromSeconds(TimeSlider.Value).ToString(@"mm\:ss");
        }
    }
}
