"use client";

import { usePathname } from "next/navigation";
import Link from "next/link";

type Props = {
  href: string;
  label: string;
};

export default function NavLink({ href, label }: Props) {
  const pathname = usePathname();
  return (
    <Link 
      key={href} 
      href={href}
      className={pathname === href ? 
        "text-yellow-400" 
        : "hover:text-yellow-400/80"}
    >
      {label}
    </Link>
  );
}
