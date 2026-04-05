import axios from "axios";
import React, { useEffect } from "react";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { IoIosStar } from "react-icons/io";

function MyFavouriteRecipes()
{
        const [recipes, setRecipes] = useState([]);
        const [success, setSuccess] = useState();
        const [error, setError] = useState();

        var navigate = useNavigate();
        
        var apiUrl = import.meta.env.VITE_API_URL;
        var token = localStorage.getItem("token") || sessionStorage.getItem("token");

        const handleRecipes = async () => {
                        
                try
                {
                        const response = await axios.get(`${apiUrl}/api/profile/my_favourite_recipes`, {
                                headers: {
                                        Authorization: `Bearer ${token}`,       
                                },
                        });

                        setRecipes(Array.isArray(response.data) ? response.data : response.data.recipes || []);
                }

                catch(ex)
                {
                        setError(ex.response);
                }
        }

        const handleToggle = async (id) => {

                try
                {
                        const response = await axios.post(`${apiUrl}/api/profile/toggle_favourite`, {Id: id, Status: false}, {headers: {Authorization: `Bearer ${token}`}});
                        handleRecipes();
                }

                catch
                {

                }
        }

        useEffect(() => {

                handleRecipes();

        }, [apiUrl, token]);

        return (
                <>
                        <span className="m-4 font-semibold text-lg">My favourite recipes</span>

                        <div className="flex flex-wrap justify-center max-h-64 overflow-y-auto gap-2">

                        {recipes.map((recipe, index) => (
                        <div key={index} className="flex flex-col bg-amber-300 p-4 rounded-2xl w-64 m-2">
                        <button className="bg-amber-100 w-8 p-2 rounded-lg" onClick={(e) => handleToggle(recipe.id)}><IoIosStar color="gold" /></button>
                                <button className="text-lg font-semibold text-gray-800" onClick={(e) => navigate(`/recipe/${recipe.id}`)}>
                                        {recipe.title}
                                </button>

                                <div className="flex justify-center items-center mt-3">
                                        <span className="text-gray-700 m-2 text-sm">
                                                {recipe.carma} Carma
                                        </span>
                                        <span>
                                                {recipe.author}
                                        </span>
                                </div>

                        </div>
                        ))}
                        </div>
                </>
        );

}

export default MyFavouriteRecipes;