import React, { useEffect } from "react";
import SideBar from "../SideBarComponent";
import { MdEmail } from "react-icons/md";
import { useNavigate } from "react-router-dom";

function ActivateMessage()
{
        const navigate = useNavigate();

        const verifyUsers = sessionStorage.getItem("activateUser");

        useEffect(() => {

                if (!verifyUsers) {
                        navigate("/login");
                }

        }, [verifyUsers, navigate]);

        const handleBack = () => {
                sessionStorage.removeItem("activateUser");
                navigate("/login");
        }

        return (
                <div className="flex flex-col h-screen items-center m-4">
                
                        <SideBar />

                        <div className="flex flex-1 w-full items-center justify-center">
                                
                                <div className="flex flex-col items-center justify-center bg-amber-300 p-2 m-2 rounded-lg w-96">
                                        <span className="font-semibold text-xl text-gray-800">Activate your account</span>
                                        <span className="font-semibold text-gray-800 text-center">We will send an email to your email address with an activation link.</span>
                                        <MdEmail size={40} />

                                        <button className="bg-amber-500 rounded-lg p-2 m-2 font-semibold text-gray-800" onClick={handleBack}>Back to login</button>
                                </div>

                        </div>
                
                </div>
        );
}

export default ActivateMessage;