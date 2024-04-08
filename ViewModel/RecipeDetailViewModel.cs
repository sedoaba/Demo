using MvvmHelpers;
using System.ComponentModel;
using RecipeDemo.Model;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Runtime.CompilerServices;

namespace RecipeDemo.ViewModel
{
    public class RecipeDetailViewModel : BaseViewModel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        private string _strInstructions;

        public string StrInstructions
        {
            get { return _strInstructions; }
            set
            {
                if (_strInstructions != value)
                {
                    _strInstructions = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _strMealThumb;

        public string StrMealThumb
        {
            get { return _strMealThumb; }
            set
            {
                if (_strMealThumb != value)
                {
                    _strMealThumb = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _strMeal;

        public string StrMeal
        {
            get { return _strMeal; }
            set
            {
                if (_strMeal != value)
                {
                    _strMeal = value;
                    OnPropertyChanged();
                }
            }
        }

        public RecipeDetailViewModel(Meal meal)
        {
            Task.Run(async () => await GetRecipeAync(meal.idMeal));
        }

        public async Task GetRecipeAync(string idMeal)
        {
            if(IsBusy) return;

            IsBusy = true;

            try
            {
                Uri uri = new($"https://www.themealdb.com/api/json/v1/1/lookup.php?i={idMeal}");

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
                    var mealResult = JsonSerializer.Deserialize<ObservableCollection<Meal>>(root.GetProperty("meals").ToString(), options);

                    StrMeal = mealResult.First().strMeal;
                    StrMealThumb = mealResult.First().strMealThumb;
                    StrInstructions = mealResult.First().strInstructions;
                }
            }
            catch(Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Alert",ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
        public void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
