import {CarFront} from "lucide-react";

export default function NavBar() {
    return (
        <header className='sticky top-0 z-50 p-3 bg-background shadow-lg flex items-center justify-between'>
            <div className='flex items-center gap-2 text-3xl font-semibold text-red-500'>
                <CarFront size={50} />
                <div>Carsties Auctions</div>
            </div>
            <div>Search</div>
            <div>Login</div>
        </header>
    );
}