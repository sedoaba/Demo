using Microsoft.Maui.Controls;
using MvvmHelpers;
using RecipeDemo.Model;
using RecipeDemo.View;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;
using System.Xml.Linq;

namespace RecipeDemo.ViewModel
{
    public class RecipeLandingViewModel : BaseViewModel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<Category> _recipeCategoryList;

        public ObservableCollection<Category> RecipeCategoryList
        {
            get { return _recipeCategoryList; }
            set
            {
                if (_recipeCategoryList != value)
                {
                    _recipeCategoryList = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<Meal> _recipeMealList;

        public ObservableCollection<Meal> RecipeMealList
        {
            get { return _recipeMealList; }
            set
            {
                if (_recipeMealList != value)
                {
                    _recipeMealList = value;
                    OnPropertyChanged();
                }
            }
        }

        INavigation _navigation;

        public Meal SelectedItem { get; set; }

        public RecipeLandingViewModel(INavigation navigation)
        {
            _navigation = navigation;
        }

        public ICommand SelectedRecipeCommand => new Command<Object>((Object e) =>
        {
            _navigation.PushAsync(new RecipeDetailPage(SelectedItem), true);
        });

        public async Task GetRecipeCategoryListAsync()
        {
            if(IsBusy) return;

            IsBusy = true;

            try
            {
                Uri uri = new("https://www.themealdb.com/api/json/v1/1/categories.php");

                HttpClient client = new();
                HttpResponseMessage response = await client.GetAsync(uri);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var jsonDocument = JsonDocument.Parse(content);
                    var root = jsonDocument.RootElement;
                    RecipeCategoryList = JsonSerializer.Deserialize<ObservableCollection<Category>>(root.GetProperty("categories").ToString(), options);
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Alert", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task GetRecipeListAsync(string filter)
        {
            if (IsBusy) return;

            IsBusy = true;

            try
            {
                Uri uri = new($"https://www.themealdb.com/api/json/v1/1/filter.php?c={filter}");

                HttpClient client = new();
                HttpResponseMessage response = await client.GetAsync(uri);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var jsonDocument = JsonDocument.Parse(content);
                    var root = jsonDocument.RootElement;
                    RecipeMealList = JsonSerializer.Deserialize<ObservableCollection<Meal>>(root.GetProperty("meals").ToString(), options);
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Alert", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
