import type { PropsWithChildren } from "react";

interface ButtonProps
{    
    className: string;
    onClick?: () => any;
}

export default function Button({ onClick, className, children } : ButtonProps & PropsWithChildren)
{
    return (
        <button className={"p-2 rounded-sm text-white text-sm " + className} onClick={onClick}>{children}</button>
    );
}