import { Card, CardHeader, Input, Button } from "@heroui/react";
import { GiPadlock } from "react-icons/gi";

export default function LoginForm() {
    return (
        <Card className="w-md shadow-xl">
            <CardHeader className="flex flex-col items-center justify-center">
                <div className="flex flex-col gap-2 items-center">
                    <div className="flex flex-row items-center gap-3">
                        <GiPadlock size={30}/>
                        <h1 className="text-3xl font-semibold">Login</h1>
                    </div>
                
                    <p className="text-foreground/60">Welcome back to NextMatch</p>
                </div>
            </CardHeader>

            <form className="flex flex-col gap-4 px-6 py-4">
                <Input 
                    type="email"
                    placeholder="Enter your email"
                />
                <Input 
                    type="password"
                    placeholder="Enter your password"
                />
                <Button type="submit" className="w-full">
                    Sign in
                </Button>
            </form>
        </Card>
    )
}