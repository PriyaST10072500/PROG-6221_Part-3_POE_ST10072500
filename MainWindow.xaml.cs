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

        //Gets and Sets the Steps
        public ObservableCollection<Steps> Steps { get; set; }

        //Gets and Sets the Recipe
        public ObservableCollection<Recipe> Recipes { get; set; }

        private Recipe orgRecipe;

        private CaloriesAmount calAmount;

        public MainWindow()
        {
            InitializeComponent();
            Ingredients = new ObservableCollection<Ingredient>();
            Steps = new ObservableCollection<Steps>();
            IngredientsItemsControl.ItemsSource = Ingredients;
            StepsItemsControl.ItemsSource = Steps;

            Recipes = new ObservableCollection<Recipe>();
            AllRecipesListBox.ItemsSource = Recipes;

            calAmount = new CaloriesAmount(300, 0); // Calorie limit set to 300
            calAmount.CaloriesExceeded += CaloriesAmount_CaloriesExceeded;

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
            Steps.Clear();
        }


        // Add Button
        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var newRecipe = new Recipe
            {
                RecipeName = RecipeNameTextBox.Text,
                Ingredients = new ObservableCollection<Ingredient>(Ingredients),
                Steps = new ObservableCollection<Steps>(Steps)
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
            calAmount.Credit(totalCalories);

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

                RecipeDetailsTextBox.AppendText("\nSteps:\n");
                foreach (var instruction in foundRecipe.Steps)
                {
                    RecipeDetailsTextBox.AppendText($"{instruction.RecipeSteps}\n");
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

                ScaledRecipeTextBox.AppendText("\nSteps:\n");
                foreach (var instruction in foundRecipe.Steps)
                {
                    ScaledRecipeTextBox.AppendText($"{instruction.RecipeSteps}\n");
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

                    ScaledRecipeTextBox.AppendText("\nSteps:\n");
                    foreach (var instruction in foundRecipe.Steps)
                    {
                        ScaledRecipeTextBox.AppendText($"{instruction.RecipeSteps}\n");
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


        //Reset Button
        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {
            string searchQuery = ScaleRecipeTextBox.Text.Trim();
            var foundRecipe = Recipes.FirstOrDefault(r => r.RecipeName.Equals(searchQuery, StringComparison.OrdinalIgnoreCase));

            if (foundRecipe != null && orgRecipe != null)
            {
                // Original inputs
                foundRecipe.Ingredients = new ObservableCollection<Ingredient>(orgRecipe.Ingredients.Select(i => new Ingredient
                {
                    IngredientQuantity = i.IngredientQuantity,
                    IngredientUnit = i.IngredientUnit,
                    IngredientName = i.IngredientName
                }));

                foundRecipe.Steps = new ObservableCollection<Steps>(orgRecipe.Steps.Select(ins => new Steps
                {
                    RecipeSteps = ins.RecipeSteps
                }));

                // Update the text in the ScaledRecipeTextBox to display the original recipe
                ScaledRecipeTextBox.Text = $"Recipe Name: {foundRecipe.RecipeName}\n\n";
                ScaledRecipeTextBox.AppendText("Ingredients:\n");

                foreach (var ingredient in foundRecipe.Ingredients)
                {
                    ScaledRecipeTextBox.AppendText($"{ingredient.IngredientQuantity} {ingredient.IngredientUnit} {ingredient.IngredientName}\n");
                }

                ScaledRecipeTextBox.AppendText("\nSteps:\n");
                foreach (var instruction in foundRecipe.Steps)
                {
                    ScaledRecipeTextBox.AppendText($"{instruction.RecipeSteps}\n");
                }

                // Update tabs by refreshing 
                AllRecipesListBox.Items.Refresh();

                // Reset the scaled number
                foundRecipe.IsScaled = false;

                
                ScaleBtn.IsEnabled = true;
                ScalingFactorComboBox.IsEnabled = true;
            }
            else
            {
                ScaledRecipeTextBox.Text = "Recipe not found!";
            }
        }



        private void FilterRecipes(string FilteringredientName, string FilterfoodGroup, double FiltermaxCalories)
        {
            var filteredRecipes = new ObservableCollection<Recipe>();

            foreach (var recipe in Recipes)
            {
                if (recipe.MatchesFilter(FilteringredientName, FilterfoodGroup, FiltermaxCalories))
                {
                    filteredRecipes.Add(recipe);
                }
            }

            AllRecipesListBox.ItemsSource = filteredRecipes; // Updates the ListBox 
        }


        // Filter Button
        private void FilterBtn_Click(object sender, RoutedEventArgs e)
        {
            string FilteringredientName = FilterIngredientTextBox.Text.Trim();
            string FilterfoodGroup = (FilterFoodGroupComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            double FiltermaxCalories = 0;

            if (double.TryParse(FilterMaxCaloriesTextBox.Text, out double maxCalories))
            {
                FiltermaxCalories = maxCalories;
            }

            FilterRecipes(FilteringredientName, FilterfoodGroup, FiltermaxCalories);
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
                MessageBox.Show("Enter recipe name to be deleted.", "Fill Recipe Name", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        // Add Ingredient Button
        private void AddIngredientBtn_Click(object sender, RoutedEventArgs e)
        {
            Ingredients.Add(new Ingredient());
        }

        private void AddInstructionBtn_Click(object sender, RoutedEventArgs e)
        {
            Steps.Add(new Steps());
        }


    }
}