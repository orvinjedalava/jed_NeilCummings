export { cn } from "cn"

export function debounce<F extends (...args: Parameters<F>) => ReturnType<F>>(
    func: F,
    waitFor: number,
){
    let timeout: ReturnType<typeof setTimeout>;

    const debounced = (...args: Parameters<F>): void => {
        clearTimeout(timeout);
        timeout = setTimeout(() => func(...args), waitFor);
    };
    
    debounced.cancel = () => clearTimeout(timeout);
    
    return debounced;
}
