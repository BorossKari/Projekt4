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
using System.IO;

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
        int looptype = 0;
        int loopend = 0;
        int readinggood = 0;
        int temprandom = -1;
        DispatcherTimer timer = new DispatcherTimer();
        MediaPlayer mediaPlayer = new MediaPlayer();
        public MainWindow()
        {
            InitializeComponent();
            mediaPlayer.Volume = 0.25;
            PlayList.SelectedIndex = -1;
            mediaPlayer.MediaEnded += new EventHandler(SongEndedLooping);
        }
        private void SongEndedLooping(object sender, EventArgs e)
        {
            Looping();
        }
        public void Looping()
        {
            if (slideruse != 1)
            {
                if (looptype == 0)
                {
                    if (playedsong != PlayList.Items.Count - 1)
                    {
                        playedsong++;
                        PlayList.SelectedIndex = playedsong;
                        mediaPlayer.Open(new Uri(Convert.ToString(PlayList.Items[playedsong])));
                        mediaPlayer.Play();
                        mediaPlayer.MediaFailed += Media_Error;
                    }
                    else
                    {
                        mediaPlayer.Pause();
                        playing = 0;
                        PlayPauseButton.Background = (Brush)new BrushConverter().ConvertFrom("#FF3169FF");
                        loopend = 1;
                        TimeSlider.IsEnabled = false;
                    }
                }
                else if (looptype == 1)
                {
                    if (playedsong != PlayList.Items.Count - 1)
                    {
                        playedsong++;
                        PlayList.SelectedIndex = playedsong;
                        mediaPlayer.Open(new Uri(Convert.ToString(PlayList.Items[playedsong])));
                        mediaPlayer.Play();
                        mediaPlayer.MediaFailed += Media_Error;
                    }
                    else if (playedsong == PlayList.Items.Count - 1)
                    {
                        playedsong = 0;
                        PlayList.SelectedIndex = playedsong;
                        mediaPlayer.Open(new Uri(Convert.ToString(PlayList.Items[playedsong])));
                        mediaPlayer.Play();
                        mediaPlayer.MediaFailed += Media_Error;
                    }
                }
                else if (looptype == 2)
                {
                    mediaPlayer.Open(new Uri(Convert.ToString(PlayList.Items[playedsong])));
                    mediaPlayer.Play();
                    mediaPlayer.MediaFailed += Media_Error;
                }
                else if (looptype == 3)
                {
                    RandomSong();
                }
            }
        }
        public void RandomSong()
        {
            Random rando = new Random();
            do
            {
                temprandom = rando.Next(0, PlayList.Items.Count);
            } while (temprandom == playedsong && PlayList.Items.Count != 1);
            mediaPlayer.Open(new Uri(Convert.ToString(PlayList.Items[temprandom])));
            playedsong = temprandom;
            PlayList.SelectedIndex = playedsong;
            temprandom = -1;
            mediaPlayer.Play();
            mediaPlayer.MediaFailed += Media_Error;
        }
        public void InitiateStop()
        {
            mediaPlayer.Stop();
            playing = 0;
            fullstop = 1;
            playedsong = -1;
            PlayList.SelectedIndex = -1;
            TimeSlider.IsEnabled = false;
            TimeSlider.Value = 0;
            TimeLabel.Content = "00:00 / 00:00";
            PlayPauseButton.Background = (Brush)new BrushConverter().ConvertFrom("#FF3169FF");
            StopButton.Background = (Brush)new BrushConverter().ConvertFrom("#FFA40000");
        }
        private void Opensongs_Click(object sender, RoutedEventArgs e)
        {
            InitiateStop();
            try
            {
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
                mediaPlayer.MediaFailed += Media_Error;
                if (firstopen == 1)
                {
                    mediaPlayer.Play();
                    mediaPlayer.Stop();
                    firstopen = 0;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("An error occured while trying to open the files. Please make sure you selected valid files or didn't cancel the import then try again.", "Error");
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
                    InitiateStop();
                }
                else if (PlayList.Items.Count > 1)
                {
                    if (PlayList.SelectedIndex == playedsong)
                    {
                        PlayList.Items.Remove(PlayList.SelectedItem);
                        InitiateStop();
                    }
                    else if (PlayList.SelectedIndex < playedsong)
                    {
                        PlayList.Items.Remove(PlayList.SelectedItem);
                        playedsong--;
                        if (PlayList.SelectedIndex != -1)
                        {
                            PlayList.SelectedIndex--;
                        }
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
            InitiateStop();
        }
        private void Saveplaylist_Click(object sender, RoutedEventArgs e)
        {
            InitiateStop();
            if (PlayList.Items.Count == 0)
            {
                MessageBox.Show("You can't save an empty playlist. Please add some songs then try again.", "Error");
            }
            else
            {
                SaveFileDialog sfd = new SaveFileDialog()
                {
                    Filter = "Text files (*.txt)|*.txt"
                };
                sfd.ShowDialog();
                try
                {
                    StreamWriter sw = new StreamWriter(sfd.FileName);
                    sw.WriteLine("*/WPF MEDIA PLAYER PLAYLIST FILE BEGINNING/*");
                    for (int i = 0; i < PlayList.Items.Count; i++)
                    {
                        sw.WriteLine(PlayList.Items[i]);
                    }
                    sw.Close();
                }
                catch (Exception)
                {
                    MessageBox.Show("An error occured while trying to write the file. Please make sure you specified a valid name and that you didn't cancel the save.", "Error");
                }
            }
        }
        private void Loadplaylist_Click(object sender, RoutedEventArgs e)
        {
            if (PlayList.Items.Count == 0)
            {
                OpenFileDialog ofd = new OpenFileDialog()
                {
                    Filter = "Text files (*.txt)|*.txt"
                };
                ofd.ShowDialog();
                try
                {
                    StreamReader sr = new StreamReader(ofd.FileName);
                    if (readinggood == 0)
                    {
                        if (Convert.ToString(sr.ReadLine()) == "*/WPF MEDIA PLAYER PLAYLIST FILE BEGINNING/*")
                        {
                            readinggood = 1;
                        }
                        else
                        {
                            MessageBox.Show("An error occured while trying to read the file. Please make sure you specified a valid file and that you didn't cancel the load. Check if the file was made by this program and that its' contents haven't been changed, then try again.", "Error");
                        }
                    }
                    if (readinggood == 1)
                    {
                        do
                        {
                            PlayList.Items.Add(Convert.ToString(sr.ReadLine()));
                        }while (!sr.EndOfStream);
                        sr.Close();
                        readinggood = 0;
                        mediaPlayer.Open(new Uri(Convert.ToString(PlayList.Items[0])));
                        mediaPlayer.MediaFailed += Media_Error;
                        if (firstopen == 1)
                        {
                            mediaPlayer.Play();
                            mediaPlayer.Stop();
                            firstopen = 0;
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("An error occured while trying to read the file. Please make sure you specified a valid file and that you didn't cancel the load. Check if the file was made by this program and that its' contents haven't been changed, then try again.", "Error");
                }
            }
            else
            {
                InitiateStop();
                MessageBoxResult mbr = MessageBox.Show("Loading a playlist will erase your current one. Are you sure?", "Confirmation", MessageBoxButton.YesNo);
                if (mbr == MessageBoxResult.Yes)
                {
                    OpenFileDialog ofd = new OpenFileDialog()
                    {
                        Filter = "Text files (*.txt)|*.txt"
                    };
                    ofd.ShowDialog();
                    try
                    {
                        StreamReader sr = new StreamReader(ofd.FileName);
                        if (readinggood == 0)
                        {
                            if (Convert.ToString(sr.ReadLine()) == "*/WPF MEDIA PLAYER PLAYLIST FILE BEGINNING/*")
                            {
                                readinggood = 1;
                                firstopen = 1;
                                PlayList.Items.Clear();
                            }
                            else
                            {
                                MessageBox.Show("An error occured while trying to read the file. Please make sure you specified a valid file and that you didn't cancel the load. Check if the file was made by this program and that its' contents haven't been changed, then try again.", "Error");
                            }
                        }
                        if (readinggood == 1)
                        {
                            do
                            {
                                PlayList.Items.Add(Convert.ToString(sr.ReadLine()));
                            } while (!sr.EndOfStream);
                            sr.Close();
                            mediaPlayer.Open(new Uri(Convert.ToString(PlayList.Items[0])));
                            mediaPlayer.MediaFailed += Media_Error;
                            readinggood = 0;
                            if (firstopen == 1)
                            {
                                mediaPlayer.Play();
                                mediaPlayer.Stop();
                                firstopen = 0;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("An error occured while trying to read the file. Please make sure you specified a valid file and that you didn't cancel the load. Check if the file was made by this program and that its' contents haven't been changed, then try again.", "Error");
                    }
                }
            }
        }
        private void PlayList_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                DependencyObject obj = (DependencyObject)e.OriginalSource;
                while (obj != null && obj != PlayList)
                {
                    if (obj.GetType() == typeof(ListBoxItem))
                    {
                        fullstop = 0;
                        loopend = 0;
                        TimeSlider.IsEnabled = true;
                        mediaPlayer.Open(new Uri(Convert.ToString(PlayList.Items[PlayList.SelectedIndex])));
                        mediaPlayer.Play();
                        playing = 1;
                        playedsong = PlayList.SelectedIndex;
                        mediaPlayer.MediaFailed += Media_Error;
                        timer.Interval = TimeSpan.FromSeconds(1);
                        timer.Tick += TimerRefresher;
                        timer.Start();
                        PlayPauseButton.Background = (Brush)new BrushConverter().ConvertFrom("#FFB0C6FF");
                        StopButton.Background = (Brush)new BrushConverter().ConvertFrom("#FFFF0000");
                        break;
                    }
                    obj = VisualTreeHelper.GetParent(obj);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("An error occured while trying to play the file. Please make sure the file is valid and still in the same directory then try again.", "Error");
            }
        }
        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (fullstop == 0)
            {
                if (loopend == 0)
                {
                    if (playing == 0)
                    {
                        mediaPlayer.Play();
                        playing = 1;
                        PlayPauseButton.Background = (Brush)new BrushConverter().ConvertFrom("#FFB0C6FF");
                    }
                    else if (playing == 1)
                    {
                        mediaPlayer.Pause();
                        playing = 0;
                        PlayPauseButton.Background = (Brush)new BrushConverter().ConvertFrom("#FF3169FF");
                    }
                    StopButton.Background = (Brush)new BrushConverter().ConvertFrom("#FFFF0000");
                    timer.Interval = TimeSpan.FromSeconds(1);
                    timer.Tick += TimerRefresher;
                    timer.Start();
                }
                else if (loopend == 1)
                {
                    if (PlayList.Items.Count != 0)
                    {
                        loopend = 0;
                        playedsong = 0;
                        PlayList.SelectedIndex = playedsong;
                        TimeSlider.IsEnabled = true;
                        mediaPlayer.Open(new Uri(Convert.ToString(PlayList.Items[playedsong])));
                        mediaPlayer.Play();
                        mediaPlayer.MediaFailed += Media_Error;
                        playing = 1;
                        PlayPauseButton.Background = (Brush)new BrushConverter().ConvertFrom("#FFB0C6FF");
                    }
                }
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            InitiateStop();
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            if (fullstop == 0)
            {
                loopend = 0;
                TimeSlider.IsEnabled = true;
                if (mediaPlayer.Position.TotalSeconds < 3 || playing == 0)
                {
                    if (looptype == 3)
                    {
                        RandomSong();
                    }
                    else
                    {
                        if (playedsong == 0)
                        {
                            playedsong = PlayList.Items.Count - 1;
                            PlayList.SelectedIndex = playedsong;
                            try
                            {
                                TimeSlider.IsEnabled = true;
                                mediaPlayer.Open(new Uri(Convert.ToString(PlayList.Items[playedsong])));
                                mediaPlayer.MediaFailed += Media_Error;
                                if (playing == 1)
                                {
                                    mediaPlayer.Play();
                                    playing = 1;
                                    timer.Interval = TimeSpan.FromSeconds(1);
                                    timer.Tick += TimerRefresher;
                                    timer.Start();
                                }
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("An error occured while trying to play the file. Please make sure the file is valid and still in the same directory then try again.", "Error");
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
                                mediaPlayer.MediaFailed += Media_Error;
                                if (playing == 1)
                                {
                                    mediaPlayer.Play();
                                    playing = 1;
                                    timer.Interval = TimeSpan.FromSeconds(1);
                                    timer.Tick += TimerRefresher;
                                    timer.Start();
                                }
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("An error occured while trying to play the file. Please make sure the file is valid and still in the same directory then try again.", "Error");
                            }
                        }
                    }
                }
                else if (mediaPlayer.Position.TotalSeconds >= 3)
                {
                    mediaPlayer.Open(new Uri(Convert.ToString(PlayList.Items[playedsong])));
                    mediaPlayer.MediaFailed += Media_Error;
                    PlayList.SelectedIndex = playedsong;
                }
            }
        }
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (fullstop == 0)
            {
                loopend = 0;
                if (looptype == 3 && playing == 1)
                {
                    RandomSong();
                }
                else
                {
                    if (playedsong == PlayList.Items.Count - 1)
                    {
                        playedsong = 0;
                        PlayList.SelectedIndex = playedsong;
                        try
                        {
                            TimeSlider.IsEnabled = true;
                            mediaPlayer.Open(new Uri(Convert.ToString(PlayList.Items[playedsong])));
                            mediaPlayer.MediaFailed += Media_Error;
                            if (playing == 1)
                            {
                                mediaPlayer.Play();
                                playing = 1;
                                timer.Interval = TimeSpan.FromSeconds(1);
                                timer.Tick += TimerRefresher;
                                timer.Start();
                            }
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("An error occured while trying to play the file. Please make sure the file is valid and still in the same directory then try again.", "Error");
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
                            mediaPlayer.MediaFailed += Media_Error;
                            if (playing == 1)
                            {
                                mediaPlayer.Play();
                                playing = 1;
                                timer.Interval = TimeSpan.FromSeconds(1);
                                timer.Tick += TimerRefresher;
                                timer.Start();
                            }
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("An error occured while trying to play the file. Please make sure the file is valid and still in the same directory then try again.", "Error");
                        }
                    }
                }
            }
        }

        private void RepeatButton_Click(object sender, RoutedEventArgs e)
        {
            if (looptype == 0)
            {
                looptype = 1;
                RepeatButton.Background = (Brush)new BrushConverter().ConvertFrom("#FF28FA78");
            }
            else if(looptype == 1)
            {
                looptype = 2;
                RepeatButton.Content = "🔂";
            }
            else if (looptype == 2)
            {
                looptype = 3;
                RepeatButton.Content = "🔀";
            }
            else if (looptype == 3)
            {
                looptype = 0;
                RepeatButton.Background = (Brush)new BrushConverter().ConvertFrom("#FF9B9B9B");
                RepeatButton.Content = "🔁";
            }
        }
        private void EasterEgg_Label(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("You found an easter egg!\rYou probably like the picture.\rOr you really dislike it...","Nice job!");
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
            if (TimeSlider.Value == TimeSlider.Maximum)
            {
                Looping();
            }
        }
        private void TimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TimeLabel.Content = String.Format("{0} / {1}", TimeSpan.FromSeconds(TimeSlider.Value).ToString(@"mm\:ss"), mediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
        }

        private void VolumeChanged(object sender, RoutedPropertyChangedEventArgs<double> vol)
        {
            mediaPlayer.Volume = vol.NewValue/100;
            try
            {
                VolumeText.Text = Convert.ToString(Math.Round(vol.NewValue));
            }
            catch
            {
                //Startup-kor exception-t dob, viszont innentől nincs kihatással a programra. Nem kezelendő.
            }
        }
        private void Media_Error(object sender, EventArgs e)
        {
            PlayList.Items.RemoveAt(PlayList.SelectedIndex);
            PlayList.Items.Refresh();
            InitiateStop();
            MessageBox.Show("A playback error occured. The selected file was removed from the playlist. Please check if the file isn't damaged or is still in the same directory.", "Error");
        }
    }
}
