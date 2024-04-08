using RecipeDemo.Model;
using RecipeDemo.ViewModel;

namespace RecipeDemo.View;

public partial class RecipeDetailPage : ContentPage
{
    private RecipeDetailViewModel viewModel;

    public RecipeDetailPage(Meal meal)
	{
		InitializeComponent();

        if(meal != null )
        {
            Title = meal.strMeal;
            viewModel = new RecipeDetailViewModel(meal);
            BindingContext = viewModel;
        }
    }
}