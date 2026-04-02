import React, { useState } from "react";
import { GoSearch } from "react-icons/go";
import { CiLogin } from "react-icons/ci";

function SideBar()
{
        
        return (
                <div className="flex w-full max-w-4xl bg-green-500 p-4 rounded-lg items-center">
                
                <span>Logo</span>

                <div className="flex flex-1 justify-center gap-2">
                        <input placeholder="Enter the recipe name" className="rounded-lg bg-green-300 placeholder:text-gray-800 py-2 w-96 text-center" />
                        <button className="bg-green-300 p-2 rounded-lg"><GoSearch /></button>
                </div>

                <button className="flex flex-row bg-green-300 rounded-lg p-2 justify-center items-center gap-2 text-gray-800">
                        Login <CiLogin />
                </button>

                </div>
        )
}

export default SideBar;