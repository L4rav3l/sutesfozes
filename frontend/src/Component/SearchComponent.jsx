import axios from "axios";
import React, { useEffect, useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import SideBar from "./SideBarComponent";

function Search() {
    const [params] = useSearchParams();

    const [search, setSearch] = useState(params.get("search") || "");
    const [notFound, setNotFound] = useState(false);
    const [recipes, setRecipes] = useState([]);

    const apiUrl = import.meta.env.VITE_API_URL;
    var navigate = useNavigate();

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
        const handleSearch = async () => {
            try {
                const response = await axios.post(`${apiUrl}/api/search`, {
                    Search: search,
                });

                const data = safeParse(response.data);

                setRecipes(data);

                if (!data || data.length === 0) {
                    setNotFound(true);
                } else {
                    setNotFound(false);
                }
            } catch (err) {
                setRecipes([]);
                setNotFound(true);
            }
        };

        if (search) {
            handleSearch();
        }
    }, [search, apiUrl]);

    return (
        <div className="flex flex-col h-screen items-center m-4">
            <SideBar />

            <div className="flex flex-1 w-full justify-center p-6">
                <div className="flex flex-col w-[32rem] gap-4">

                    {!notFound && recipes.length > 0 && (
                        <div className="flex flex-col gap-3">
                            {recipes.map((recipe, index) => (
                                <button
                                    key={index}
                                    className="bg-amber-200 p-4 rounded-lg transition"
                                    onClick={() => navigate(`/recipe/${recipe.id}`)}
                                >
                                    <div className="text-xl font-bold text-gray-800 text-center">
                                        {recipe.title}
                                    </div>
                                </button>
                            ))}
                        </div>
                    )}

                    {notFound && (
                        <div className="bg-amber-300 rounded-lg p-3 text-center">
                            <span className="font-semibold text-gray-700">
                                Not found.
                            </span>
                        </div>
                    )}

                </div>
            </div>
        </div>
    );
}

export default Search;