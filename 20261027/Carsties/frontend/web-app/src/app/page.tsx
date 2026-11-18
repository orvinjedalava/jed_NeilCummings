import Listings from "@/features/listings/Listings";
import {AppPagination} from "@/components/ui/app-pagination";

export default function Home() {
  return (
    <div>
      <Listings/>
      <AppPagination/>
    </div>
  );
}
