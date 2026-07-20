const KEY_BASE64 = import.meta.env.VITE_ENCRYPTION_KEY as string;

function base64ToBytes(base64: string): Uint8Array {
    const binary = atob(base64);
    const bytes = new Uint8Array(binary.length);
    for (let i = 0; i < binary.length; i++) bytes[i] = binary.charCodeAt(i);
    return bytes;
}

function bytesToBase64(bytes: Uint8Array): string {
    let binary = '';
    bytes.forEach((b) => (binary += String.fromCharCode(b)));
    return btoa(binary);
}

async function getKey(): Promise<CryptoKey> {
    return crypto.subtle.importKey('raw', base64ToBytes(KEY_BASE64), { name: 'AES-GCM' }, false, [
        'encrypt',
        'decrypt',
    ]);
}

export async function encryptPayload(data: unknown): Promise<string> {
    const key = await getKey();
    const iv = crypto.getRandomValues(new Uint8Array(12));
    const plaintext = new TextEncoder().encode(JSON.stringify(data));

    const ciphertextBuffer = await crypto.subtle.encrypt({ name: 'AES-GCM', iv }, key, plaintext);
    // Web Crypto appends the 16-byte auth tag to the ciphertext automatically — matches AesGcm's layout.
    const combined = new Uint8Array(iv.length + ciphertextBuffer.byteLength);
    combined.set(iv, 0);
    combined.set(new Uint8Array(ciphertextBuffer), iv.length);

    return bytesToBase64(combined);
}

export async function decryptPayload<T>(encryptedBase64: string): Promise<T> {
    const key = await getKey();
    const combined = base64ToBytes(encryptedBase64);
    const iv = combined.slice(0, 12);
    const ciphertextWithTag = combined.slice(12);

    const plaintextBuffer = await crypto.subtle.decrypt({ name: 'AES-GCM', iv }, key, ciphertextWithTag);
    const plaintext = new TextDecoder().decode(plaintextBuffer);
    return JSON.parse(plaintext) as T;
}