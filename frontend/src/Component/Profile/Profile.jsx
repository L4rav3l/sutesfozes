import React, {useEffect, useEffectEvent, useState} from "react";
import { useNavigate } from "react-router-dom";
import axios from "axios";
import SideBar from "../SideBarComponent";

import MyProfile from "./ProfileComponent/MyProfile";
import MyRecipes from "./ProfileComponent/MyRecipe";

function Profile()
{

        const [page, setPage] = useState(0);

        const [username, setUsername] = useState("");
        const [email, setEmail] = useState("");
        
        const apiUrl = import.meta.env.VITE_API_URL;
        const navigate = useNavigate();

        const token = localStorage.getItem("token") || sessionStorage.getItem("token");

        useEffect(() => {

                const token = localStorage.getItem("token") || sessionStorage.getItem("token");

                const handleGet = async () => {
                        try
                        {
                                const response = await axios.get(`${apiUrl}/api/profile/data`, {
                                        headers: {
                                                Authorization: `Bearer ${token}`,
                                        },
                                });

                                setUsername(response.data.username);
                                setEmail(response.data.email);

                                setPage(1);
                                
                        }
                        catch
                        {
                                sessionStorage.clear();
                                localStorage.clear();
                                navigate("/login");
                        }
                }

                handleGet();

        }, [apiUrl]);

        return (
                <div className="flex flex-col h-screen items-center m-4">
                        <SideBar />

                        <div className="flex flex-1 w-full items-center justify-center">

                        <div className="bg-amber-300 rounded-lg p-2 m-2">
                                <div className="bg-amber-200 p-2 w-40 rounded-lg">
                                        <ul className="flex flex-col gap-2">
                                                <button onClick={(e) => setPage(1)}>Your profile</button>
                                                <button onClick={(e) => setPage(2)}>My Recipes</button>
                                                <li>Favourite Recipes</li>
                                                <li>Submit Recipe</li>
                                                <li>Logout</li>
                                        </ul>
                                </div>
                        </div>

                        <div className="flex w-full max-w-2xl bg-amber-300 p-2 rounded-lg gap-8">

                                <div className="flex-1 bg-amber-200 p-2 rounded-lg min-h-[200px]">

                                        {page === 1 && (
                                                <MyProfile initialUsername={username} initialEmail={email} />
                                        )}

                                        {page === 2 && (
                                                <MyRecipes />
                                        )}

                                </div>

                        </div>

                        </div>
                </div>
        );
}

export default Profile;