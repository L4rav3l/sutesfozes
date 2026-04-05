import axios from "axios";
import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import SideBar from "./SideBarComponent";

import { IoIosStar } from "react-icons/io";

function Recipe()
{
        const { id } = useParams();
        
        const [title, setTitle] = useState("");
        
        const [prepTime, setPrepTime] = useState();
        const [cookTime, setCookTime] = useState();
        const [serve, setServe] = useState();

        const [ingredients, setIngredients] = useState([]);
        const [instructions, setInstructions] = useState("");

        const [images, setImages] = useState([]);

        const [favourite, setFavourite] = useState();

        const [notFound, setNotFound] = useState();

        var apiUrl = import.meta.env.VITE_API_URL;
        var token = localStorage.getItem("token") || sessionStorage.getItem("token");

        const safeParse = (data) => {
                if (!data) return [];

                if (typeof data === "object") return data;

                try {
                        return JSON.parse(data);
                } catch {
                        return [];
                }
        };

        useEffect(() => {

                const handleRecipes = async () => {
                        try
                        {
                                const response = await axios.get(`${apiUrl}/api/recipes?Id=${id}`);

                                const data = response.data;

                                setTitle(data.title);
                                setPrepTime(data.prepTime);
                                setCookTime(data.cookTime);
                                setServe(data.serve);

                                setIngredients(JSON.parse(data.ingredients));
                                setInstructions(data.instructions);
                                
                                setImages(safeParse(data.images));
                        }

                        catch(ex)
                        {

                                setNotFound(true);
                                alert("error");
                        }
                }

                const handleFavourite = async () => {
                        try
                        {
                                const response = await axios.get(`${apiUrl}/api/profile/favourite?Id=${id}`, {headers: {Authorization: `Bearer ${token}`}});

                                setFavourite(response.data.status);
                        }
                        catch
                        {
                                setFavourite(false);
                        }
                }

                handleRecipes();
                handleFavourite();

        }, [apiUrl, id]);
        
        const handleToggle = async (status) => {
                        try
                        {
                                const response = await axios.post(`${apiUrl}/api/profile/toggle_favourite`, {Id: id, Status: status}, {headers: {Authorization: `Bearer ${token}`}});

                                setFavourite(status);
                        }
                        catch
                        {
                                
                        }
        }

        return (
                <div className="flex flex-col h-screen items-center m-4">
                
                        <SideBar />

<div className="flex flex-1 w-full items-center justify-center p-6">
    <div className="flex flex-col bg-amber-200 w-[32rem] p-6 rounded-2xl shadow-lg gap-4">
        
        <div className="flex bg-amber-100 p-2 w-12 rounded-lg justify-center items-center">
                
                {!favourite && (
                        <button onClick={() => handleToggle(true)}>
                                <IoIosStar size="20" color="gray"/>
                        </button>
                )}

                {favourite && (
                        <button onClick={() => handleToggle(false)}>
                                <IoIosStar size="20" color="gold"/>
                        </button>
                )}
        </div>

        <span className="text-2xl font-bold text-gray-800 text-center">
            {title}
        </span>

        <div className="flex flex-col items-center text-gray-700 text-sm gap-1">
            <span>⏱ Prep time: {prepTime} min</span>
            <span>Cook time: {cookTime} min</span>
            <span>Servings: {serve}</span>
        </div>

                <div className="bg-white rounded-xl p-4 shadow-inner">
                        <h2 className="font-semibold text-gray-700 mb-2">
                                Ingredients
                        </h2>

                                <div className="flex flex-col gap-2">
                                        {ingredients.map((group, key) => (
                                        <div key={key} className="border-b pb-2 last:border-none">
                                                
                                                <div className="font-semibold text-gray-800">
                                                {group.title}
                                                </div>

                                                <div className="ml-3 text-gray-600 text-sm flex flex-col gap-1">
                                                {group.ingredients.map((item, i) => (
                                                        <div key={i} className="flex gap-2">
                                                        <span className="font-medium">
                                                                {item.name}
                                                        </span>
                                                        <span>
                                                                {item.amount} {item.unit}
                                                        </span>
                                                        </div>
                                                ))}
                                                </div>

                                        </div>
                                        ))}
                                </div>
                        </div>

                                {images.length > 0 && (
                                <div className="bg-white rounded-xl p-4 shadow-inner">
                                        <h2 className="font-semibold text-gray-700 mb-2">
                                        Images
                                        </h2>

                                        <div className="grid grid-cols-2 gap-2">
                                        {images.map((img, key) => (
                                                <img
                                                key={key}
                                                src={img}
                                                alt={`recipe-${key}`}
                                                className="w-full h-32 object-cover rounded-lg hover:scale-105 transition"
                                                />
                                        ))}
                                        </div>
                                </div>
                                )}

                                <div className="bg-white rounded-xl p-4 shadow-inner">
                                <h2 className="font-semibold text-gray-700 mb-2">
                                        Instructions
                                </h2>

                                <p className="text-gray-600 whitespace-pre-line">
                                        {instructions}
                                </p>
                                </div>

                        </div>
                        </div>
                </div>
        )


}

export default Recipe;