using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROG_6221_Part_3_POE_ST10072500
{

    public class Ingredient
    {
        //Gets and Sets the IngredientName
        public string IngredientName { get; set; }

        //Gets and Sets the IngredientQuantity
        public string IngredientQuantity { get; set; }

        //Gets and Sets the IngredientUnit
        public string IngredientUnit { get; set; }

        //Gets and Sets the IngredientCalorieCount
        public string IngredientCalorieCount { get; set; }

        //Gets and Sets the IngredientFoodGroup
        public string IngredientFoodGroup { get; set; }
    }

    public class Descriptions
    {
        //Gets and Sets the RecipeDescription
        public string RecipeDescriptions { get; set; }

    }

    public class Recipe
    {
        //Gets and Sets the RecipeName
        public string RecipeName { get; set; }

        //Gets and Sets the Ingredient
        public ObservableCollection<Ingredient> Ingredients { get; set; }

        //Gets and Sets the Description
        public ObservableCollection<Descriptions> Descriptions { get; set; }

        //Gets and Sets the Scale
        public bool IsScaled { get; set; }

        public Recipe()
        {
            Ingredients = new ObservableCollection<Ingredient>();
            Descriptions = new ObservableCollection<Descriptions>();
        }


        public bool MatchesFilter(string FilteringredientName, string FilterfoodGroup, double FiltermaxCalories)
        {
            // Check if recipe has a specific ingredient name
            if (!string.IsNullOrEmpty(FilteringredientName))
            {
                bool containsIngredient = Ingredients.Any(i => i.IngredientName.Equals(FilteringredientName, StringComparison.OrdinalIgnoreCase));
                if (!containsIngredient)
                    return false;
            }

            // Check if recipe belongs to a specific food group
            if (!string.IsNullOrEmpty(FilterfoodGroup))
            {
                bool belongsToFoodGroup = Ingredients.Any(i => i.IngredientFoodGroup.Equals(FilterfoodGroup, StringComparison.OrdinalIgnoreCase));
                if (!belongsToFoodGroup)
                    return false;
            }

            // Check if total calories of recipe are within a specific limit
            if (FiltermaxCalories > 0)
            {
                double totalCalories = Ingredients.Sum(i =>
                {
                    double.TryParse(i.IngredientCalorieCount, out double calorieCount);
                    return calorieCount;
                });

                if (totalCalories > FiltermaxCalories)
                    return false;
            }

            return true;
        }


    }

}

