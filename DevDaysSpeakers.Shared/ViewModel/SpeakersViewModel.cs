using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using DevDaysSpeakers.Model;
using DevDaysSpeakers.Services;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace DevDaysSpeakers.ViewModel
{
	public class SpeakersViewModel : INotifyPropertyChanged
	{
		public ObservableCollection<Speaker> Speakers { get; set; }
		public Command GetSpeakersCommand { get; set; }

		public SpeakersViewModel()
		{
			Speakers = new ObservableCollection<Speaker>();
			GetSpeakersCommand = new Command(
				async () => await GetSpeakers(),
				() => !IsBusy);
		}

		private bool _isBusy;

		public bool IsBusy
		{
			get
			{
				return _isBusy;
			}

			set
			{
				if (_isBusy == value) return;

				_isBusy = value;
				OnPropertyChanged();
				GetSpeakersCommand.ChangeCanExecute();
			}
		}






		private async Task GetSpeakers()
		{
			if (IsBusy) return;

			Exception error = null;

			try
			{
				IsBusy = true;

				using (var client = new HttpClient())
				{
					var jsonString = await client.GetStringAsync("http://demo4404797.mockable.io/speakers");

					var items = JsonConvert.DeserializeObject<List<Speaker>>(jsonString);
					Speakers.Clear();

					foreach (var item in items)
					{
						Speakers.Add(item);
					}
				}
				
			}
			catch (Exception ex)
			{
				error = ex;
				await Application.Current.MainPage.DisplayAlert("Error!", error.Message, "OK");
			}
			finally
			{
				IsBusy = false;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		void OnPropertyChanged([CallerMemberName] string name = null)
		{
			var changed = PropertyChanged;

			if (changed == null)
				return;

			changed.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
