import axios from "axios";
import React, { useEffect, useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import SideBar from "../SideBarComponent";

function VerifyEmail()
{
        const [success, setSuccess] = useState("");
        const [tried, setTried] = useState(false);
        

        const [params] = useSearchParams();

        const apiUrl = import.meta.env.VITE_API_URL;
        const navigate = useNavigate();

        useEffect(() => {
                const t = params.get("token");
                        
                if(!t)
                {
                        navigate("/login");
                } else {

                const handleVerify = async () => {
                        try
                        {
                                const response = await axios.post(`${apiUrl}/api/profile/verify_email`, {Token: t});
                                sessionStorage.clear();
                                localStorage.clear();

                                setSuccess(true);
                        }
                        catch
                        {
                                setSuccess(false);
                        }

                        setTried(true);
                };

                handleVerify();
        }}, [params, navigate, apiUrl]);

        const handleBack = () => {
                navigate("/login");
        }

        return (
                <div className="flex flex-col h-screen items-center m-4">
                        <SideBar />                             
                        
                        {tried && (
                                success ? (
                                        <div className="flex flex-1 w-full items-center justify-center">
                                        
                                        <div className="flex flex-col items-center justify-center bg-amber-300 p-2 m-2 rounded-lg w-96">
                                                <span className="font-semibold text-xl text-gray-800 text-center">Your account email address was changed.</span>
                                                <span className="font-semibold text-gray-800 text-center">Your account email address was changed. Now you need to log in again.</span>

                                                <button className="bg-amber-500 rounded-lg p-2 m-2 font-semibold text-gray-800" onClick={handleBack}>Back to login</button>
                                        </div>

                                        </div>
                                ) : (

                                        <div className="flex flex-1 w-full items-center justify-center">
                                        
                                        <div className="flex flex-col items-center justify-center bg-amber-300 p-2 m-2 rounded-lg w-96">
                                                <span className="font-semibold text-xl text-gray-800 text-center">Your account email address wasn't changed.</span>
                                                <span className="font-semibold text-gray-800 text-center">Your token has expired or is invalid. Please try again.</span>

                                                <button className="bg-amber-500 rounded-lg p-2 m-2 font-semibold text-gray-800" onClick={handleBack}>Back to login</button>
                                        </div>

                                        </div>
                                )
                        )}

                        {!tried && (
                                <>
                                        
                                </>
                        )}
                </div>
        );
}

export default VerifyEmail;