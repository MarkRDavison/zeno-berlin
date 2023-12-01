export interface Config {
    bff_base_uri: string
}

declare global {
    interface Window {
        ENV: {
            BFF_BASE_URI: string
        };
    }
}

const createConfig = (): Config => {
    return {
        bff_base_uri: window.ENV?.BFF_BASE_URI ?? 'https://localhost:40000',
    }
};

const config = createConfig();

export default config;