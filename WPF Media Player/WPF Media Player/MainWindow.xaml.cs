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
        int fullstop = 1;
        int playedsong = -1;
        int firstopen = 1;
        DispatcherTimer timer = new DispatcherTimer();
        MediaPlayer mediaPlayer = new MediaPlayer();
        public MainWindow()
        {
            InitializeComponent();
            mediaPlayer.Volume = 0.25;
            PlayList.SelectedIndex = -1;
        }
        private void Opensongs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                mediaPlayer.Stop();
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "MP3 files (*.mp3)|*.mp3",
                    Multiselect = true
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    foreach (string files in openFileDialog.FileNames)
                    {
                        if (!PlayList.Items.Contains(files))
                        {
                            PlayList.Items.Add(files);
                        }
                    }
                }
                mediaPlayer.Open(new Uri(Convert.ToString(PlayList.Items[0])));
                if (firstopen == 1)
                {
                    mediaPlayer.Play();
                    mediaPlayer.Stop();
                    firstopen = 0;
                }
            }
            catch
            {

            }
        }
        private void PlayList_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (PlayList.Items.Count != 0)
                {
                    fullstop = 0;
                    TimeSlider.IsEnabled = true;
                    mediaPlayer.Open(new Uri(Convert.ToString(PlayList.Items[PlayList.SelectedIndex])));
                    mediaPlayer.Play();
                    playing = 1;
                    playedsong = PlayList.SelectedIndex;
                    timer.Interval = TimeSpan.FromSeconds(1);
                    timer.Tick += TimerRefresher;
                    timer.Start();
                }
            }
            catch
            {

            }
        }
        private void Removesong_Click(object sender, RoutedEventArgs e)
        {
            if (PlayList.Items.Count == 0 || PlayList.SelectedIndex == -1)
            {

            }
            else
            {
                if (PlayList.Items.Count == 1)
                {
                    PlayList.Items.Remove(PlayList.SelectedItem);
                    mediaPlayer.Stop();
                    playing = 0;
                    fullstop = 1;
                    PlayList.SelectedIndex = -1;
                    TimeSlider.IsEnabled = false;
                    TimeSlider.Value = 0;
                    TimeLabel.Content = "00:00 / 00:00";
                }
                else if (PlayList.Items.Count > 1)
                {
                    if (PlayList.SelectedIndex == playedsong)
                    {
                        PlayList.Items.Remove(PlayList.SelectedItem);
                        mediaPlayer.Stop();
                        playing = 0;
                        fullstop = 1;
                        PlayList.SelectedIndex = -1;
                        TimeSlider.IsEnabled = false;
                        TimeSlider.Value = 0;
                        TimeLabel.Content = "00:00 / 00:00";
                    }
                    else if (PlayList.SelectedIndex < playedsong)
                    {
                        PlayList.Items.Remove(PlayList.SelectedItem);
                        playedsong--;
                        PlayList.SelectedIndex--;
                    }
                    else if (PlayList.SelectedIndex > playedsong)
                    {
                        PlayList.Items.Remove(PlayList.SelectedItem);
                    }
                }
            }
        }
        private void ClearPlaylist_Click(object sender, RoutedEventArgs e)
        {
            PlayList.Items.Clear();
            mediaPlayer.Stop();
            playing = 0;
            fullstop = 1;
            playedsong = -1;
            PlayList.SelectedIndex = -1;
            TimeSlider.IsEnabled = false;
            TimeSlider.Value = 0;
            TimeLabel.Content = "00:00 / 00:00";
        }

        private void Saveplaylist_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Loadplaylist_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (fullstop == 0)
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
                }
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += TimerRefresher;
                timer.Start();
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
            playing = 0;
            fullstop = 1;
            playedsong = -1;
            PlayList.SelectedIndex = -1;
            TimeSlider.IsEnabled = false;
            TimeSlider.Value = 0;
            TimeLabel.Content = "00:00 / 00:00";
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            if (fullstop == 0)
            {
                if (mediaPlayer.Position.TotalSeconds < 3)
                {
                    if (playedsong == 0)
                    {
                        playedsong = PlayList.Items.Count - 1;
                        PlayList.SelectedIndex = playedsong;
                        try
                        {
                            TimeSlider.IsEnabled = true;
                            mediaPlayer.Open(new Uri(Convert.ToString(PlayList.Items[playedsong])));
                            if (playing == 1)
                            {
                                mediaPlayer.Play();
                                playing = 1;
                                timer.Interval = TimeSpan.FromSeconds(1);
                                timer.Tick += TimerRefresher;
                                timer.Start();
                            }
                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        playedsong -= 1;
                        PlayList.SelectedIndex = playedsong;
                        try
                        {
                            TimeSlider.IsEnabled = true;
                            mediaPlayer.Open(new Uri(Convert.ToString(PlayList.Items[playedsong])));
                            if (playing == 1)
                            {
                                mediaPlayer.Play();
                                playing = 1;
                                timer.Interval = TimeSpan.FromSeconds(1);
                                timer.Tick += TimerRefresher;
                                timer.Start();
                            }
                        }
                        catch
                        {

                        }
                    }
                }
                else
                {
                    mediaPlayer.Open(new Uri(Convert.ToString(PlayList.Items[playedsong])));
                    PlayList.SelectedIndex = playedsong;
                }
            }
        }
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (fullstop == 0)
            {
                if (playedsong == PlayList.Items.Count - 1)
                {
                    playedsong = 0;
                    PlayList.SelectedIndex = playedsong;
                    try
                    {
                        TimeSlider.IsEnabled = true;
                        mediaPlayer.Open(new Uri(Convert.ToString(PlayList.Items[playedsong])));
                        if (playing == 1)
                        {
                            mediaPlayer.Play();
                            playing = 1;
                            timer.Interval = TimeSpan.FromSeconds(1);
                            timer.Tick += TimerRefresher;
                            timer.Start();
                        }
                    }
                    catch
                    {

                    }
                }
                else
                {
                    playedsong += 1;
                    PlayList.SelectedIndex = playedsong;
                    try
                    {
                        TimeSlider.IsEnabled = true;
                        mediaPlayer.Open(new Uri(Convert.ToString(PlayList.Items[playedsong])));
                        if (playing == 1)
                        {
                            mediaPlayer.Play();
                            playing = 1;
                            timer.Interval = TimeSpan.FromSeconds(1);
                            timer.Tick += TimerRefresher;
                            timer.Start();
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }

        private void RepeatButton_Click(object sender, RoutedEventArgs e)
        {

        }
        void TimerRefresher(object sender, EventArgs e)
        {
            if (mediaPlayer.Source != null && (mediaPlayer.NaturalDuration.HasTimeSpan) && slideruse == 0 && fullstop == 0)
            {
                TimeLabel.Content = String.Format("{0} / {1}", mediaPlayer.Position.ToString(@"mm\:ss"), mediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
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
            TimeLabel.Content = String.Format("{0} / {1}", TimeSpan.FromSeconds(TimeSlider.Value).ToString(@"mm\:ss"), mediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
        }

        private void VolumeChanged(object sender, RoutedPropertyChangedEventArgs<double> vol)
        {
            mediaPlayer.Volume = vol.NewValue;
        }
    }
}
