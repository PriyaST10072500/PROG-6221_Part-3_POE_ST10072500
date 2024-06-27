using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PROG_6221_Part_3_POE_ST10072500
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Gets and Sets the Ingredient
        public ObservableCollection<Ingredient> Ingredients { get; set; }

        //Gets and Sets the Description
        public ObservableCollection<Descriptions> Descriptions { get; set; }

        //Gets and Sets the Recipe
        public ObservableCollection<Recipe> Recipes { get; set; }

        private Recipe originalRecipe;

        private CaloriesAmount caloriesAmount;

        public MainWindow()
        {
            InitializeComponent();
            Ingredients = new ObservableCollection<Ingredient>();
            Descriptions = new ObservableCollection<Descriptions>();
            IngredientsItemsControl.ItemsSource = Ingredients;
            InstructionsItemsControl.ItemsSource = Descriptions;

            Recipes = new ObservableCollection<Recipe>();
            AllRecipesListBox.ItemsSource = Recipes;

            caloriesAmount = new CaloriesAmount(300, 0); // Calorie limit set to 300
            caloriesAmount.CaloriesExceeded += CaloriesAmount_CaloriesExceeded;

        }


        private void CaloriesAmount_CaloriesExceeded(object sender, EventArgs e)
        {
            MessageBox.Show("Calorie total exceeds 300!", "Calorie Limit Is Exceeded", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void SortRecipesAlphabetically()
        {
            var sortedRecipes = new ObservableCollection<Recipe>(Recipes.OrderBy(r => r.RecipeName));
            Recipes.Clear();
            foreach (var recipe in sortedRecipes)
            {
                Recipes.Add(recipe);
            }
        }

        private void ClearForm()
        {
            RecipeNameTextBox.Clear();
            Ingredients.Clear();
            Descriptions.Clear();
        }


        // Add Button
        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var newRecipe = new Recipe
            {
                RecipeName = RecipeNameTextBox.Text,
                Ingredients = new ObservableCollection<Ingredient>(Ingredients),
                Descriptions = new ObservableCollection<Descriptions>(Descriptions)
            };

            // Calculate the total calories for new recipe
            double totalCalories = 0;
            foreach (var ingredient in newRecipe.Ingredients)
            {
                if (double.TryParse(ingredient.IngredientCalorieCount, out double calorieCount) && double.TryParse(ingredient.IngredientQuantity, out double ingQuan))
                {
                    totalCalories += calorieCount;
                }
                else
                {
                    MessageBox.Show($"Invalid quantity: {ingredient.IngredientName}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            // Update the CaloriesAmount 
            caloriesAmount.Credit(totalCalories);

            // Add new recipe to the collection of recipes 
            Recipes.Add(newRecipe);

            // Additional operations 
            SortRecipesAlphabetically();
            ClearForm();
        }


        // Source Button
        private void SrcBtn_Click(object sender, RoutedEventArgs e)
        {
            string searchQuery = SearchRecipeTextBox.Text.Trim();
            var foundRecipe = Recipes.FirstOrDefault(r => r.RecipeName.Equals(searchQuery, StringComparison.OrdinalIgnoreCase));

            if (foundRecipe != null)
            {
                RecipeDetailsTextBox.Text = $"Recipe Name: {foundRecipe.RecipeName}\n\n";
                double totalCalories = 0;

                RecipeDetailsTextBox.AppendText("Ingredients:\n");

                foreach (var ingredient in foundRecipe.Ingredients)
                {
                    RecipeDetailsTextBox.AppendText($"{ingredient.IngredientQuantity} {ingredient.IngredientUnit} \t {ingredient.IngredientName} \t {ingredient.IngredientCalorieCount}\n");
                    double totCal = double.Parse(ingredient.IngredientCalorieCount);
                    totalCalories += totCal;
                }

                RecipeDetailsTextBox.AppendText($"\nTotal Calories: {totalCalories}\n");

                RecipeDetailsTextBox.AppendText("\nDescriptions:\n");
                foreach (var description in foundRecipe.Descriptions)
                {
                    RecipeDetailsTextBox.AppendText($"{description.RecipeDescriptions}\n");
                }
            }
            else
            {
                RecipeDetailsTextBox.Text = "Recipe not found.";
            }

        }



        // View Button
        private void ViewBtn_Click(object sender, RoutedEventArgs e)
        {
            string searchQuery = ScaleRecipeTextBox.Text.Trim();
            var foundRecipe = Recipes.FirstOrDefault(r => r.RecipeName.Equals(searchQuery, StringComparison.OrdinalIgnoreCase));

            if (foundRecipe != null)
            {
                ScaledRecipeTextBox.Text = $"Recipe Name: {foundRecipe.RecipeName}\n\n";
                double totalCalories = 0;

                ScaledRecipeTextBox.AppendText("Ingredients:\n");

                foreach (var ingredient in foundRecipe.Ingredients)
                {
                    ScaledRecipeTextBox.AppendText($"{ingredient.IngredientQuantity} {ingredient.IngredientUnit} \t {ingredient.IngredientName} \t {ingredient.IngredientCalorieCount}\n");
                    double totCal = double.Parse(ingredient.IngredientCalorieCount);
                    totalCalories += totCal;
                }

                ScaledRecipeTextBox.AppendText($"\nTotal Calories: {totalCalories}\n");

                ScaledRecipeTextBox.AppendText("\nDescriptions:\n");
                foreach (var description in foundRecipe.Descriptions)
                {
                    ScaledRecipeTextBox.AppendText($"{description.RecipeDescriptions}\n");
                }
            }
            else
            {
                ScaledRecipeTextBox.Text = "Recipe not found.";
            }
        }


        // Scale Button
        private void ScaleBtn_Click(object sender, RoutedEventArgs e)
        {
            string searchQuery = ScaleRecipeTextBox.Text.Trim();
            if (double.TryParse((ScalingFactorComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(), out double scalingFactor))
            {
                var foundRecipe = Recipes.FirstOrDefault(r => r.RecipeName.Equals(searchQuery, StringComparison.OrdinalIgnoreCase));

                if (foundRecipe != null && !foundRecipe.IsScaled) // Check if recipe scaled
                {
                    ScaledRecipeTextBox.Text = $"Recipe Name: {foundRecipe.RecipeName} (Scaled by {scalingFactor})\n\n";
                    ScaledRecipeTextBox.AppendText("Ingredients:\n");

                    foreach (var ingredient in foundRecipe.Ingredients)
                    {
                        if (double.TryParse(ingredient.IngredientQuantity, out double originalQuantity))
                        {
                            double scaledQuantity = originalQuantity * scalingFactor;
                            ingredient.IngredientQuantity = scaledQuantity.ToString();
                        }
                        ScaledRecipeTextBox.AppendText($"{ingredient.IngredientQuantity} {ingredient.IngredientUnit} {ingredient.IngredientName}\n");
                    }

                    ScaledRecipeTextBox.AppendText("\nDescriptions:\n");
                    foreach (var description in foundRecipe.Descriptions)
                    {
                        ScaledRecipeTextBox.AppendText($"{description.RecipeDescriptions}\n");
                    }

                    // Update the tabs 
                    AllRecipesListBox.Items.Refresh();

                    // Recipe is scaled
                    foundRecipe.IsScaled = true;
                }
                else if (foundRecipe != null)
                {
                    ScaledRecipeTextBox.Text = "The recipe has been scaled. Please reset it and try again.";
                }
                else
                {
                    ScaledRecipeTextBox.Text = "Recipe not found.";
                }
            }
            else
            {
                ScaledRecipeTextBox.Text = "Invalid scaling factor.";
            }
        }


        private void FilterRecipes(string ingredientNameFilter, string foodGroupFilter, double maxCaloriesFilter)
        {
            var filteredRecipes = new ObservableCollection<Recipe>();

            foreach (var recipe in Recipes)
            {
                if (recipe.MatchesFilter(ingredientNameFilter, foodGroupFilter, maxCaloriesFilter))
                {
                    filteredRecipes.Add(recipe);
                }
            }

            AllRecipesListBox.ItemsSource = filteredRecipes; // Updates the ListBox 
        }


        // Filter Button
        private void FilterBtn_Click(object sender, RoutedEventArgs e)
        {
            string ingredientNameFilter = FilterIngredientTextBox.Text.Trim();
            string foodGroupFilter = (FilterFoodGroupComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            double maxCaloriesFilter = 0;

            if (double.TryParse(FilterMaxCaloriesTextBox.Text, out double maxCalories))
            {
                maxCaloriesFilter = maxCalories;
            }

            FilterRecipes(ingredientNameFilter, foodGroupFilter, maxCaloriesFilter);
        }


        // Reset Filter Button
        private void ResetFilterBtn_Click(object sender, RoutedEventArgs e)
        {
            // Clear filter inputs and reset ListBox
            FilterIngredientTextBox.Clear();
            FilterFoodGroupComboBox.SelectedIndex = -1;
            FilterMaxCaloriesTextBox.Clear();
            AllRecipesListBox.ItemsSource = Recipes; // Reset to original list
        }



        // Delete Button
        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            string recipeNameToDelete = DeleteRecipeTextBox.Text.Trim();

            if (!string.IsNullOrEmpty(recipeNameToDelete))
            {
                Recipe recipeToDelete = Recipes.FirstOrDefault(r => r.RecipeName.Equals(recipeNameToDelete, StringComparison.OrdinalIgnoreCase));

                if (recipeToDelete != null)
                {
                    // Prompt the user
                    MessageBoxResult result = MessageBox.Show($"Do you want to delete recipe '{recipeNameToDelete}'?",
                                                              "Confirm Delete",
                                                              MessageBoxButton.YesNo,
                                                              MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        Recipes.Remove(recipeToDelete);
                        ClearForm(); // Clear the form 
                        MessageBox.Show($"Recipe '{recipeNameToDelete}' deleted successfully.", "Recipe Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                }
                else
                {
                    MessageBox.Show($"Recipe '{recipeNameToDelete}' not found.", "Recipe Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please enter the recipe's name to delete.", "Fill Recipe Name", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        // Add Ingredient Button
        private void AddIngredientBtn_Click(object sender, RoutedEventArgs e)
        {
            Ingredients.Add(new Ingredient());
        }

        private void AddInstructionBtn_Click(object sender, RoutedEventArgs e)
        {
            Descriptions.Add(new Descriptions());
        }


    }
}