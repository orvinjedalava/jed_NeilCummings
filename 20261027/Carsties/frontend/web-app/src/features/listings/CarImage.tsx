'use client'

import Image from "next/image";
import {useState} from "react";
import clsx from "clsx";

type Props = {
    imageUrl: string;
}

export default function CarImage({ imageUrl }: Props) {
    const [isLoading, setIsLoading] = useState(true);
    
    return (
        <Image
            src={imageUrl}
            alt='Image of car'
            height={400}
            width={400}
            className={clsx('object-cover duration-700 ease-in-out aspect-16/10 border-b border-muted-foreground/20', {
                'opacity-0 scale-110': isLoading,
                'opacity-100 scale-100': !isLoading
            })}
            priority
            sizes='(max-width:768px) 100vw, (max-width: 1200px) 50vw, 33vw'
            onLoad={() => setIsLoading(false)}
        />
    );
}