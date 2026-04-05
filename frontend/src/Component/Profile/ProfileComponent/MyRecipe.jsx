import axios from "axios";
import React, { useEffect } from "react";
import { useState } from "react";

function MyRecipes()
{
        const [recipe, setRecipe] = useState("");
        
        var apiUrl = import.meta.env.VITE_API_URL;
        var token = localStorage.getItem("token") || sessionStorage.getItem("token");

        useEffect(() => {

                const handleRecipes = async () => {
                        
                }

        }, [axios, apiUrl, token]);

}

export default MyRecipes;