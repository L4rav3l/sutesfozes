import axios from "axios";
import React, { useEffect } from "react";
import { useState } from "react";
import { useNavigate } from "react-router-dom";

function MyRecipes()
{
        const [recipes, setRecipes] = useState([]);
        const [success, setSuccess] = useState();
        const [error, setError] = useState();

        var navigate = useNavigate();
        
        var apiUrl = import.meta.env.VITE_API_URL;
        var token = localStorage.getItem("token") || sessionStorage.getItem("token");

        useEffect(() => {

                const handleRecipes = async () => {
                        
                        try
                        {
                                const response = await axios.get(`${apiUrl}/api/profile/my_recipes`, {
                                        headers: {
                                                Authorization: `Bearer ${token}`,       
                                        },
                                });

                                setRecipes(Array.isArray(response.data) ? response.data : response.data.recipes || []);
                        }

                        catch(ex)
                        {
                                setError(ex.response.data.error);
                        }
                }

                handleRecipes();

        }, [axios, apiUrl, token]);

        console.log(recipes);

        return (
                <>
                        <span className="m-4 font-semibold text-lg">My recipes</span>

                        <div className="flex flex-wrap justify-center max-h-64 overflow-y-auto gap-2">

                        {recipes.map((recipe, index) => (
                        <button key={index} className="flex flex-col bg-amber-300 p-4 rounded-2xl w-64 m-2" onClick={(e) => navigate(`/recipe/${recipe.id}`)}>

                                <span className="text-lg font-semibold text-gray-800">
                                {recipe.title}
                                </span>

                                <div className="flex justify-center items-center mt-3">
                                        <span className="text-gray-700 m-2 text-sm">
                                                {recipe.carma} Carma
                                        </span>

                                        {recipe.checked && (
                                        <span className="inline-block bg-green-500 text-white text-sm font-medium px-3 py-1 rounded-full">
                                                Checked
                                        </span>
                                        )}

                                        {!recipe.checked && (
                                        <span className="inline-block bg-red-500 text-white text-sm font-medium px-3 py-1 rounded-full">
                                                Unchecked
                                        </span>
                                        )}
                                </div>

                        </button>
                        ))}
                        </div>
                </>
        );

}

export default MyRecipes;