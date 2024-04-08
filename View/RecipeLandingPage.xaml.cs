using CommunityToolkit.Maui.Behaviors;
using Microsoft.Maui.Controls;
using RecipeDemo.ViewModel;

namespace RecipeDemo.View;

public partial class RecipeLandingPage : ContentPage
{
    private RecipeLandingViewModel viewModel;
    string currentValue = string.Empty;
    public RecipeLandingPage()
    {
		InitializeComponent();
        viewModel = new RecipeLandingViewModel(Navigation);

        BindingContext = viewModel;
        LoadCategories();
    }

    protected async void LoadCategories()
    {
        await viewModel.GetRecipeCategoryListAsync();

        if(viewModel.RecipeCategoryList != null)
        {
           var radioButton = RecipeCategorySection.Children[0] as RadioButton;
           radioButton.IsChecked = true;
           string currentValue = radioButton.Value.ToString();
           await viewModel.GetRecipeListAsync(currentValue);
        }
    }

    private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        var selectedCategory = sender as RadioButton;

        if(currentValue != selectedCategory.Value.ToString())
        {
           currentValue = selectedCategory.Value.ToString();
           Task.Run(async () => await viewModel.GetRecipeListAsync(currentValue));
        }
    }
}