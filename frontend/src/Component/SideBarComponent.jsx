import React, { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { GoSearch } from "react-icons/go";
import { CiLogin } from "react-icons/ci";
import { CgProfile } from "react-icons/cg";
import axios from "axios";

function SideBar()
{
        const [logged, setLogged] = useState(false);
        const [recipeName, setRecipeName] = useState("");
        const apiUrl = import.meta.env.VITE_API_URL;
        const navigate = useNavigate();
        const location = useLocation();

        const [username, setUsername] = useState("");

        useEffect(() => {
                const token = localStorage.getItem("token") || sessionStorage.getItem("token");
                const path = location.pathname;

                const handleAuth = async () => {
                        if (!token) {
                        setLogged(false);
                        if (path === "/profile") navigate("/login");
                        return;
                        }

                        try {
                        const response = await axios.post(
                                `${apiUrl}/api/authentication/verify_token`,
                                { Token: token }
                        );

                        if (response.data?.status !== 1) {
                                throw new Error("Invalid token");
                        }

                        setLogged(true);

                        if (path === "/login" || path === "/register") {
                                navigate("/profile");
                        }

                        const profileRes = await axios.get(
                                `${apiUrl}/api/profile/data`,
                                {
                                headers: {
                                        Authorization: `Bearer ${token}`,
                                },
                                }
                        );

                        setUsername(profileRes.data.username);

                        } catch (ex) {
                        sessionStorage.clear();
                        localStorage.clear();
                        setLogged(false);

                        if (path !== "/login") {
                                navigate("/login");
                        }
                        }
                };

                handleAuth();
                }, [apiUrl, location.pathname, navigate]);

        return (
                <div className="flex w-full max-w-4xl bg-green-500 p-4 rounded-lg items-center">
                
                <span>Logo</span>

                <div className="flex flex-1 justify-center gap-2">
                        <input placeholder="Enter the recipe name" className="rounded-lg bg-green-300 placeholder:text-gray-800 py-2 w-96 text-center" value={recipeName} onChange={(e) => setRecipeName(e.target.value)}/>
                        <button className="bg-green-300 p-2 rounded-lg" onClick={() => navigate(`/search?search=${recipeName}`)}><GoSearch /></button>
                </div>

                {logged ? (<button className="flex flex-row bg-green-300 rounded-lg p-2 justify-center items-center gap-2 text-gray-800" onClick={() => navigate("/profile")}>
                        {username} <CgProfile />
                </button>
                ) : (
                <button className="flex flex-row bg-green-300 rounded-lg p-2 justify-center items-center gap-2 text-gray-800" onClick={() => navigate("/login")}>
                        Login <CiLogin />
                </button>
                )}

                </div>
        )
}

export default SideBar;